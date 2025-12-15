using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using POS_TranVietTraLam_Fresher_BLL.Constants;
using POS_TranVietTraLam_Fresher_BLL.Defines;
using POS_TranVietTraLam_Fresher_BLL.DTO.AuthDTO;
using POS_TranVietTraLam_Fresher_DAL.Defines;
using POS_TranVietTraLam_Fresher_Entities.Entity;
using System.Security.Claims;

namespace POS_TranVietTraLam_Fresher_BLL.Implements
{
    internal class RegistrationInfo
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }

    internal class RegistrationTempStore
    {
        private static readonly MemoryCache Cache = new MemoryCache(new MemoryCacheOptions());

        public void Set(string email, string token, RegistrationInfo info, TimeSpan ttl)
        {
            Cache.Set(GetKey(email, token), info, ttl);
        }

        public RegistrationInfo? Get(string email, string token)
        {
            Cache.TryGetValue(GetKey(email, token), out RegistrationInfo? info);
            return info;
        }

        public void Remove(string email, string token)
        {
            Cache.Remove(GetKey(email, token));
        }

        private static string GetKey(string email, string token) => $"reg:{email}:{token}";
    }
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly IOTPService _otpService;
        private readonly ILogger<AuthService> _logger;
        private static readonly RegistrationTempStore RegistrationTempStore = new RegistrationTempStore();

        public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService, IOTPService otpService, ILogger<AuthService> logger)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _otpService = otpService;
            _logger = logger;
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request)
        {
            var user = await _unitOfWork.UserRepository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                throw new UnauthorizedAccessException(AuthMessage.LOGIN_USER_NOT_FOUND);
            }

            bool isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!isValidPassword)
            {
                throw new UnauthorizedAccessException(AuthMessage.LOGIN_FAILED);
            }

            if (user.IsActive != 1)
            {
                throw new UnauthorizedAccessException(AuthMessage.LOGIN_USER_NOT_ACTIVE);
            }

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken(user);

            _logger.LogInformation("User logged in successfully: {Email}", request.Email);

            return new LoginResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<RegisterResponseDTO> RegisterAsync(RegisterRequestDTO request)
        {
            var existingUser = await _unitOfWork.UserRepository.GetByEmailAsync(request.Email);

            if (existingUser != null)
            {
                throw new InvalidOperationException(AuthMessage.REGISTER_EMAIL_EXISTS);
            }

            var registrationToken = Guid.NewGuid().ToString("N");
            RegistrationTempStore.Set(request.Email, registrationToken, new RegistrationInfo
            {
               PasswordHash = HashPassword(request.Password)
            }, TimeSpan.FromMinutes(15));

            var otpResult = await _otpService.SendOTPAsync(request.Email, "REGISTER");
            if (!otpResult.Success)
            {
                throw new InvalidOperationException(otpResult.Message);
            }

            _logger.LogInformation("OTP sent for registration: {Email}", request.Email);

            return new RegisterResponseDTO
            {
                Email = request.Email,
                Message = "Vui lòng kiểm tra email và nhập mã xác thực để hoàn tất đăng ký.",
                RegistrationToken = registrationToken
            };
        }

        public async Task<LoginResponseDTO> RefreshTokenAsync(string refreshToken)
        {
            var claimsPrincipal = _jwtService.ValidateRefreshToken(refreshToken);

            if (claimsPrincipal == null)
            {
                throw new UnauthorizedAccessException(AuthMessage.REFRESH_INVALID_TOKEN);
            }

            var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                throw new UnauthorizedAccessException(AuthMessage.REFRESH_USER_NOT_FOUND);
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync(userGuid);

            if (user == null)
            {
                throw new UnauthorizedAccessException(AuthMessage.REFRESH_USER_NOT_FOUND);
            }

            if (user.IsActive != 1)
            {
                throw new UnauthorizedAccessException(AuthMessage.LOGIN_USER_NOT_ACTIVE);
            }

            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken(user);

            _logger.LogInformation("Token refreshed successfully for user: {Email}", user.Email);

            return new LoginResponseDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task<bool> CreateUserAfterOTPVerificationAsync(string email, string otpCode, string registrationToken)
        {
            var otpResponse = await _otpService.VerifyOTPAsync(email, otpCode, "REGISTER");
            if (!otpResponse.Success)
            {
                throw new InvalidOperationException(otpResponse.Message);
            }

            var registrationInfo = RegistrationTempStore.Get(email, registrationToken);
            if (registrationInfo == null)
            {
                throw new InvalidOperationException("Thông tin đăng ký đã hết hạn hoặc không hợp lệ.");
            }

            var existingUser = await _unitOfWork.UserRepository.GetByEmailAsync(email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Tài khoản đã tồn tại.");
            }

            var newUser = new User
            {
                UserId = Guid.NewGuid(),
                Email = email,
                PasswordHash = registrationInfo.PasswordHash,
                Role = "User",
                IsActive = 1,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.UserRepository.AddAsync(newUser);
            await _unitOfWork.Save();

            _logger.LogInformation("User created successfully after OTP verification: {Email}", email);

            RegistrationTempStore.Remove(email, registrationToken);
            return true;
        }
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
