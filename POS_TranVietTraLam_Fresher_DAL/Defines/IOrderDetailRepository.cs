using POS_TranVietTraLam_Fresher_Entities.Entity;

namespace POS_TranVietTraLam_Fresher_DAL.Defines
{
    public interface IOrderDetailRepository : IGenericRepository<OrderDetail>
    {
        Task<Order?> GetOrderWithDetailsAsync(int orderId);
    }
}
