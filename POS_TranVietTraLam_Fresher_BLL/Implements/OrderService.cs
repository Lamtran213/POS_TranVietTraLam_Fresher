using Microsoft.Extensions.Options;
using POS_TranVietTraLam_Fresher_BLL.Defines;
using POS_TranVietTraLam_Fresher_BLL.DTO.CommonDTO;
using POS_TranVietTraLam_Fresher_BLL.DTO.OrderDTO;
using POS_TranVietTraLam_Fresher_DAL.Defines;
using POS_TranVietTraLam_Fresher_Entities.Entity;
using POS_TranVietTraLam_Fresher_Entities.Enum;
using static POS_TranVietTraLam_Fresher_BLL.DTO.PayosDTO.PayosDTOs;

namespace POS_TranVietTraLam_Fresher_BLL.Implements
{
    public class OrderService : IOrderService
    {
        private readonly IAuthenticatedUser _authenticatedUser;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPayosService _payosService;
        private readonly PayOSSettings _payosSettings;
        public OrderService(IAuthenticatedUser authenticatedUser, IUnitOfWork unitOfWork, IPayosService payosService, IOptions<PayOSSettings> payOSSettings)
        {
            _authenticatedUser = authenticatedUser;
            _unitOfWork = unitOfWork;
            _payosService = payosService;
            _payosSettings = payOSSettings.Value;
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

        public async Task<CreateOrderResponseDTO> CreateOrderFromCart(CreateOrderRequestDTO request)
        {
            var cartItems = await _unitOfWork.CartItemRepository
                .GetByListIdsAsync(request.CartItemIds);

            if (!cartItems.Any())
                throw new InvalidOperationException("Cart is empty");

            var isOnlinePayment = request.PaymentMethod == PaymentMethod.PayOS;

            // ===== CREATE ORDER =====
            var order = new Order
            {
                UserId = _authenticatedUser.UserId,
                OrderDate = DateTime.UtcNow,
                RequiredDate = DateTime.UtcNow.AddDays(5),
                Freight = request.Freight,
                TotalAmount = request.TotalAmount,
                IsPaid = false,
                OrderStatus = OrderStatus.Pending,
                Address = request.Address,
                OrderDetails = cartItems.Select(ci => new OrderDetail
                {
                    ProductId = ci.ProductId,
                    UnitPrice = (decimal)ci.Product!.UnitPrice,
                    Quantity = ci.Quantity,
                    Discount = ci.Product.Discount
                }).ToList()
            };

            await _unitOfWork.OrderRepository.AddAsync(order);
            await _unitOfWork.Save();

            // ===== CREATE PAYMENT =====
            var payment = new Payment
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                Amount = order.TotalAmount,
                Method = request.PaymentMethod,
                Status = PaymentStatus.Pending
            };

            if (request.PaymentMethod == PaymentMethod.COD)
            {
                order.OrderStatus = OrderStatus.Shipping;
                payment.Status = PaymentStatus.Pending;
            }

            await _unitOfWork.PaymentRepository.AddAsync(payment);

            await _unitOfWork.OrderRepository.UpdateAsync(order);
            await _unitOfWork.Save();

            // ===== PAYOS =====
            PayOSPaymentResponse? payosResponse = null;

            if (isOnlinePayment)
            {
                payosResponse = await _payosService.CreatePaymentLinkAsync(new CreatePayOSPaymentRequest
                {
                    OrderCode = payment.PaymentId,
                    Amount = order.TotalAmount,
                    Description = $"Thanh toán đơn hàng #{order.OrderId}",
                    ReturnUrl = _payosSettings.ReturnUrl,
                    CancelUrl = _payosSettings.CancelUrl,
                    Items = order.OrderDetails.Select(d => new PayOSItem
                    {
                        Name = d.Product!.ProductName,
                        Quantity = d.Quantity,
                        Price = d.UnitPrice
                    }).ToList()
                });

                payment.PayosOrderCode = payosResponse.OrderCode;
                await _unitOfWork.PaymentRepository.UpdateAsync(payment);
                await _unitOfWork.Save();
            }

            // ===== DELETE CART =====
            await _unitOfWork.CartItemRepository.DeleteByListIdsAsync(request.CartItemIds);
            await _unitOfWork.Save();

            // ===== RESPONSE =====
            return new CreateOrderResponseDTO
            {
                OrderId = order.OrderId,
                PaymentUrl = isOnlinePayment ? payosResponse?.CheckoutUrl : null,
                RedirectUrl = isOnlinePayment
                    ? null
                    : $"/order/success?orderId={order.OrderId}",
                IsPaid = false,
                PaymentStatus = payment.Status
            };
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
            OrderStatus? status, int pageIndex, int pageSize)
        {
            var orders = await _unitOfWork.OrderRepository.GetAllWithDetailsAsync();
            if (orderDate != DateTime.MinValue)
            {
                orders = orders.Where(o => o.OrderDate.Date == orderDate.Date);
            }

            if (status != OrderStatus.All)
            {
                orders = orders.Where(o => o.OrderStatus == status);
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
