using POS_TranVietTraLam_Fresher_Entities.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_TranVietTraLam_Fresher_DAL.Defines
{
    public interface ICartItemRepository : IGenericRepository<CartItem>
    {
        Task<List<CartItem>> GetByListIdsAsync(List<int> cartItemIds);
        Task<bool> DeleteByListIdsAsync(List<int> cartItemIds);
    }
}
