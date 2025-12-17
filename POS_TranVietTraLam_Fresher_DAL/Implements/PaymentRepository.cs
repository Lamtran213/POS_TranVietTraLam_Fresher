using Microsoft.EntityFrameworkCore;
using POS_TranVietTraLam_Fresher_DAL.Context;
using POS_TranVietTraLam_Fresher_DAL.Defines;
using POS_TranVietTraLam_Fresher_Entities.Entity;
using POS_TranVietTraLam_Fresher_Entities.Enum;

namespace POS_TranVietTraLam_Fresher_DAL.Implements
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(POSContext context) : base(context)
        {
        }

        public async Task<Payment?> GetByOrderCodeAsync(int orderCode)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(p => p.PayosOrderCode == orderCode);
        }

        public async Task<List<Payment>> GetAllWithDetailsAsync()
        {
            return await _dbSet.AsNoTracking()
                .Include(p => p.User)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetPaidPaymentsWithDetailsAsync()
        {
            return await _dbSet.AsNoTracking()
                .Include(p => p.User)
                .Where(p => p.PaidAt.HasValue)
                .OrderByDescending(p => p.PaidAt)
                .ToListAsync();
        }

        public async Task MarkPaidAsync(int paymentId, DateTimeOffset paidAt)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(p => p.PaymentId == paymentId);
            if (entity == null) return;
            entity.Status = PaymentStatus.Paid;
            entity.PaidAt = paidAt.UtcDateTime;
            await _context.SaveChangesAsync();
        }

        public async Task MarkFailedAsync(int paymentId)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(p => p.PaymentId == paymentId);
            if (entity == null) return;
            entity.Status = PaymentStatus.Failed;
            await _context.SaveChangesAsync();
        }

        public async Task<string> GetEmailByUserIdAsync(Guid userId)
        {
            var payment = await _dbSet.AsNoTracking()
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);
            return payment?.User.Email ?? string.Empty;
        }
    }
}
