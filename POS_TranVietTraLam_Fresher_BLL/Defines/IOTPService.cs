using POS_TranVietTraLam_Fresher_BLL.DTO.CommonDTO;

namespace POS_TranVietTraLam_Fresher_BLL.Defines
{
    public interface IOTPService
    {
        Task<ApiResponse<bool>> SendOTPAsync(string email, string purpose);
        Task<ApiResponse<bool>> VerifyOTPAsync(string email, string otpCode, string purpose);
        Task<ApiResponse<bool>> ResendOTPAsync(string email, string purpose);
    }
}
