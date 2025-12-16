using POS_TranVietTraLam_Fresher_BLL.DTO.OrderDTO;
using POS_TranVietTraLam_Fresher_Entities.Enum;

namespace POS_TranVietTraLam_Fresher_BLL.Defines
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponseDTO?>> GetByUserId(Guid userId);
        Task<int> CreateOrderFromCart(decimal freight, decimal totalAmount, List<int> cartItemIds, string? address = null, int? voucherId = null);
        Task<OrderResponseDTO> GetById(int orderId);
        Task<bool> UpdateStatusOrder(int orderId, OrderStatus status);
        Task<string> GetCustomerEmailByMemberId(Guid memberId);
        Task<IEnumerable<OrderResponseDTO>> GetAllOrders(DateTime orderDate,
            OrderStatus status, int pageIndex, int pageSize);

        Task<int> CountOrders(DateTime orderDate, OrderStatus status);
    }
}
