using POS_TranVietTraLam_Fresher_Entities.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_TranVietTraLam_Fresher_DAL.Defines
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetBySupabaseUserIdAsync(string supabaseUserId);
    }
}
