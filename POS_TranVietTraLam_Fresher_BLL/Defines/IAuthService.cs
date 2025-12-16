using POS_TranVietTraLam_Fresher_BLL.DTO.AuthDTO;

namespace POS_TranVietTraLam_Fresher_BLL.Defines
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request);
        Task<RegisterResponseDTO> RegisterAsync(RegisterRequestDTO request);
        Task<LoginResponseDTO> GoogleLoginAsync(GoogleLoginRequestDTO request);
        Task<LoginResponseDTO> RefreshTokenAsync(string refreshToken);
        Task<bool> CreateUserAfterOTPVerificationAsync(string email, string otpCode, string registrationToken);
    }
}
