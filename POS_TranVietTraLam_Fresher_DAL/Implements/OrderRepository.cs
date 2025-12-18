using Microsoft.EntityFrameworkCore;
using POS_TranVietTraLam_Fresher_DAL.Context;
using POS_TranVietTraLam_Fresher_DAL.Defines;
using POS_TranVietTraLam_Fresher_Entities.Entity;
using POS_TranVietTraLam_Fresher_Entities.Enum;

namespace POS_TranVietTraLam_Fresher_DAL.Implements
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly POSContext _context;
        public OrderRepository(POSContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Order?> CreateNewOrder(Order newOrder)
        {
            await _context.Orders.AddAsync(newOrder);
            return newOrder;
        }

        public async Task<Order?> GetByOrderId(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<IEnumerable<Order?>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetAllWithDetailsAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ToListAsync();
        }

        public async Task MarkPaidAsync(int orderId, DateTimeOffset paidAt)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(p => p.OrderId == orderId);
            if (entity == null) return;
            entity.OrderStatus = OrderStatus.Paid;
            entity.IsPaid = true;
            entity.PaidAt = paidAt.UtcDateTime;
            await _context.SaveChangesAsync();
        }
    }
}
