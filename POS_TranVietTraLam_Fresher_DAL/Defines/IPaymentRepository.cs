using POS_TranVietTraLam_Fresher_Entities.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_TranVietTraLam_Fresher_DAL.Defines
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<Payment?> GetByOrderCodeAsync(int orderCode);
        Task<List<Payment>> GetAllWithDetailsAsync();
        Task MarkPaidAsync(int paymentId, DateTimeOffset paidAt);
        Task MarkFailedAsync(int paymentId);
        Task<List<Payment>> GetPaidPaymentsWithDetailsAsync();
    }
}
