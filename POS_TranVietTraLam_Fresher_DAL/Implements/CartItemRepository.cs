using Microsoft.EntityFrameworkCore;
using POS_TranVietTraLam_Fresher_DAL.Context;
using POS_TranVietTraLam_Fresher_DAL.Defines;
using POS_TranVietTraLam_Fresher_Entities.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_TranVietTraLam_Fresher_DAL.Implements
{
    public class CartItemRepository : GenericRepository<CartItem>, ICartItemRepository
    {
        private readonly POSContext _context;
        public CartItemRepository(POSContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<CartItem>> GetByListIdsAsync(List<int> cartItemIds)
        {
            return await _dbSet
                .Include(ci => ci.Product)
                .Where(ci => cartItemIds.Contains(ci.CartItemId))
                .ToListAsync();
        }

        public async Task<bool> DeleteByListIdsAsync(List<int> cartItemIds)
        {
            var cartItems = await _dbSet.Where(ci => cartItemIds.Contains(ci.CartItemId)).ToListAsync();
            if (cartItems.Count == 0)
            {
                return false;
            }
            _dbSet.RemoveRange(cartItems);
            return true;
        }
    }
}
