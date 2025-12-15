using Microsoft.EntityFrameworkCore;
using POS_TranVietTraLam_Fresher_DAL.Context;
using POS_TranVietTraLam_Fresher_DAL.Defines;
using POS_TranVietTraLam_Fresher_Entities.Entity;

namespace POS_TranVietTraLam_Fresher_DAL.Implements
{
    public class OTPRepository : GenericRepository<OTP>, IOTPRepository
    {
        private readonly POSContext _context;

        public OTPRepository(POSContext context) : base(context)
        {
            _context = context;
        }

        public async Task<OTP?> GetValidOTPAsync(string email, string otpCode, string purpose)
        {
            return await _context.OTPs
                .FirstOrDefaultAsync(otp =>
                    otp.Email == email &&
                    otp.OTPCode == otpCode &&
                    otp.Purpose == purpose &&
                    otp.ExpiresAt > DateTime.UtcNow &&
                    !otp.IsUsed);
        }

        public async Task<int> GetOTPCountByEmailAsync(string email, string purpose, DateTime fromTime)
        {
            return await _context.OTPs
                .CountAsync(otp =>
                    otp.Email == email &&
                    otp.Purpose == purpose &&
                    otp.CreatedAt >= fromTime);
        }

        public async Task<bool> InvalidateOTPsAsync(string email, string purpose)
        {
            var otps = await _context.OTPs
                .Where(otp => otp.Email == email && otp.Purpose == purpose && !otp.IsUsed)
                .ToListAsync();

            foreach (var otp in otps)
            {
                otp.IsUsed = true;
                otp.UsedAt = DateTime.UtcNow;
            }

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
