using Microsoft.EntityFrameworkCore;
using POS_TranVietTraLam_Fresher_DAL.Context;
using POS_TranVietTraLam_Fresher_DAL.Defines;
using POS_TranVietTraLam_Fresher_Entities.Entity;

namespace POS_TranVietTraLam_Fresher_DAL.Implements
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly POSContext _context;
        public UserRepository(POSContext context) : base(context)
        {
            _context = context;
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetBySupabaseUserIdAsync(string supabaseUserId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.SupabaseUserId == supabaseUserId);
        }
    }
}
