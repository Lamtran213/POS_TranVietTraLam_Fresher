using POS_TranVietTraLam_Fresher_BLL.Defines;
using POS_TranVietTraLam_Fresher_BLL.DTO.OrderDTO;
using POS_TranVietTraLam_Fresher_DAL.Defines;
using POS_TranVietTraLam_Fresher_Entities.Entity;
using POS_TranVietTraLam_Fresher_Entities.Enum;

namespace POS_TranVietTraLam_Fresher_BLL.Implements
{
    public class OrderService : IOrderService
    {
        private readonly IAuthenticatedUser _authenticatedUser;
        private readonly IUnitOfWork _unitOfWork;
        public OrderService(IAuthenticatedUser authenticatedUser, IUnitOfWork unitOfWork)
        {
            _authenticatedUser = authenticatedUser;
            _unitOfWork = unitOfWork;
        }
        private OrderResponseDTO MapOrderToDto(Order o)
        {
            var orderDetailDtos = o.OrderDetails.Select(od =>
            {
                var unitPrice = od.UnitPrice == 0 && od.Product != null ? (od.Product.UnitPrice ?? 0) : od.UnitPrice;
                var discount = od.Discount == 0 && od.Product != null ? od.Product.Discount : od.Discount;
                var subtotal = unitPrice * od.Quantity * (decimal)(1 - discount);

                return new { OrderDetail = od, Subtotal = subtotal, UnitPrice = unitPrice, Discount = discount };
            }).ToList();

            var totalSubtotal = orderDetailDtos.Sum(x => x.Subtotal);
            var result = new OrderResponseDTO
            {
                OrderId = o.OrderId,
                MemberId = o.UserId,
                OrderDate = o.OrderDate,
                IsPaid = o.IsPaid,
                PaidAt = o.PaidAt,
                TotalAmount = totalSubtotal,
                Address = o.Address ?? string.Empty,
                Status = (OrderStatus)o.OrderStatus,
                OrderDetailItems = orderDetailDtos.Select(x => new OrderDetailResponseDTO
                {
                    OrderDetailId = x.OrderDetail.OrderDetailId,
                    ProductId = x.OrderDetail.ProductId,
                    ProductName = x.OrderDetail.Product?.ProductName ?? string.Empty,
                    ImageUrl = x.OrderDetail.Product?.ImageUrl ?? string.Empty,
                    Quantity = x.OrderDetail.Quantity,
                    UnitPrice = x.UnitPrice,
                    Discount = x.Discount
                }).ToList()
            };
            return result;
        }
        public async Task<IEnumerable<OrderResponseDTO?>> GetByUserId(Guid userId)
        {
            var order = await _unitOfWork.OrderRepository.GetByUserIdAsync(userId);
            if (order == null)
            {
                throw new Exception("Order not found for the user.");
            }

            var result = order.Select(o => MapOrderToDto(o));
            return result;
        }

        public async Task<int> CreateOrderFromCart(decimal freight, decimal totalAmount, List<int> cartItemIds, string? address = null, int? voucherId = null)
        {
            var cartItems = await _unitOfWork.CartItemRepository.GetByListIdsAsync(cartItemIds);
            var newOrder = new Order
            {
                UserId = _authenticatedUser.UserId,
                OrderDate = DateTime.UtcNow,
                RequiredDate = DateTime.UtcNow.AddDays(5),
                Freight = freight,
                TotalAmount = totalAmount,
                IsPaid = true,
                OrderStatus = (OrderStatus)(int)OrderStatus.Paid,
                Address = address,
                OrderDetails = cartItems.Select(ci => new OrderDetail
                {
                    ProductId = ci.ProductId,
                    UnitPrice = ci.Product?.UnitPrice ?? 0m,
                    Quantity = ci.Quantity,
                    Discount = ci.Product?.Discount ?? 0
                }).ToList()
            };

            await _unitOfWork.OrderRepository.CreateNewOrder(newOrder);

            await _unitOfWork.CartItemRepository.DeleteByListIdsAsync(cartItemIds);
            foreach (var item in newOrder.OrderDetails)
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.UnitsInStock -= item.Quantity;
                    await _unitOfWork.ProductRepository.UpdateAsync(product);
                }
            }

            var saveResult = await _unitOfWork.Save();
            if (!saveResult || newOrder.OrderId == 0)
            {
                return 0;
            }

            return newOrder.OrderId;
        }

        public async Task<OrderResponseDTO> GetById(int orderId)
        {
            var order = (await _unitOfWork.OrderRepository.GetByOrderId(orderId));

            var result = MapOrderToDto(order);
            return result;
        }

        public async Task<bool> UpdateStatusOrder(int orderId, OrderStatus status)
        {
            var order = await _unitOfWork.OrderRepository.GetByOrderId(orderId);
            switch (status)
            {
                case OrderStatus.Pending:
                    order.OrderStatus = OrderStatus.Pending;
                    break;
                case OrderStatus.Paid:
                    order.OrderStatus = OrderStatus.Paid;
                    order.IsPaid = true;
                    order.PaidAt = DateTime.Now;
                    break;
                case OrderStatus.Shipping:
                    order.OrderStatus = OrderStatus.Shipping;
                    break;
                case OrderStatus.Completed:
                    order.OrderStatus = OrderStatus.Completed;
                    order.ShippedDate = DateTime.Now;
                    break;
                case OrderStatus.Cancelled:
                    order.OrderStatus = OrderStatus.Cancelled;
                    foreach (var item in order.OrderDetails)
                    {
                        var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId);
                        if (product != null)
                        {
                            product.UnitsInStock += item.Quantity;
                            await _unitOfWork.ProductRepository.UpdateAsync(product);
                        }
                    }

                    break;
            }

            await _unitOfWork.OrderRepository.UpdateAsync(order);
            return await _unitOfWork.Save();
        }

        public async Task<IEnumerable<OrderResponseDTO>> GetAllOrders(DateTime orderDate,
            OrderStatus status, int pageIndex, int pageSize)
        {
            var orders = await _unitOfWork.OrderRepository.GetAllWithDetailsAsync();
            if (orderDate != DateTime.MinValue)
            {
                orders = orders.Where(o => o.OrderDate.Date == orderDate.Date);
            }

            if (status != OrderStatus.All)
            {
                //orders = orders.Where(o => o.Status == (int)status);
            }

            var paginatedOrders = orders.Skip(pageIndex * pageSize).Take(pageSize);
            var result = paginatedOrders.Select(o => MapOrderToDto(o));
            return result;
        }

        public async Task<int> CountOrders(DateTime orderDate, OrderStatus status)
        {
            var orders = await _unitOfWork.OrderRepository.GetAllAsync();

            if (orderDate != DateTime.MinValue)
                orders = orders.Where(o => o.OrderDate.Date == orderDate.Date);

            if (status != OrderStatus.All)
                orders = orders.Where(o => o.OrderStatus == status);

            return orders.Count();
        }

        public async Task<string> GetCustomerEmailByMemberId(Guid memberId)
        {
            var member = await _unitOfWork.UserRepository.GetByIdAsync(memberId);
            if (member == null)
            {
                throw new Exception("Member not found.");
            }

            return member.Email;
        }
    }
}
