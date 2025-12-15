using POS_TranVietTraLam_Fresher_Entities.Entity;

namespace POS_TranVietTraLam_Fresher_DAL.Defines
{
    public interface IOTPRepository : IGenericRepository<OTP>
    {
        Task<OTP?> GetValidOTPAsync(string email, string otpCode, string purpose);
        Task<int> GetOTPCountByEmailAsync(string email, string purpose, DateTime fromTime);
        Task<bool> InvalidateOTPsAsync(string email, string purpose);
    }
}
