using POS_TranVietTraLam_Fresher_BLL.DTO.OrderDTO;
using POS_TranVietTraLam_Fresher_Entities.Enum;

namespace POS_TranVietTraLam_Fresher_BLL.Defines
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponseDTO?>> GetByUserId(Guid userId);
        Task<CreateOrderResponseDTO> CreateOrderFromCart(CreateOrderRequestDTO request);
        Task<OrderResponseDTO> GetById(int orderId);
        Task<bool> UpdateStatusOrder(int orderId, OrderStatus status);
        Task<string> GetCustomerEmailByMemberId(Guid memberId);
        Task<IEnumerable<OrderResponseDTO>> GetAllOrders(DateTime orderDate,
            OrderStatus? status, int pageIndex, int pageSize);

        Task<int> CountOrders(DateTime orderDate, OrderStatus status);
    }
}
