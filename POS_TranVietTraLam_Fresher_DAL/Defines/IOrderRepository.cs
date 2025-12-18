using POS_TranVietTraLam_Fresher_Entities.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_TranVietTraLam_Fresher_DAL.Defines
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<Order?>> GetByUserIdAsync(Guid userId);
        Task<Order?> CreateNewOrder(Order newOrder);
        Task<Order?> GetByOrderId(int orderId);
        Task<IEnumerable<Order>> GetAllWithDetailsAsync();
        Task MarkPaidAsync(int orderId, DateTimeOffset paidAt);
    }
}
