using Microsoft.AspNetCore.Mvc;
using POS_TranVietTraLam_Fresher_BLL.Constants;
using POS_TranVietTraLam_Fresher_BLL.Defines;
using POS_TranVietTraLam_Fresher_BLL.DTO.AuthDTO;
using POS_TranVietTraLam_Fresher_BLL.DTO.CommonDTO;

namespace POS_TranVietTraLam_Fresher_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IOTPService _otpService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthService authService, IOTPService otpService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _otpService = otpService;
            _logger = logger;
        }
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponseDTO>>> Login([FromBody] LoginRequestDTO request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);

                return StatusCode(ResponseCodes.StatusCodes.OK, new ApiResponse<LoginResponseDTO>
                {
                    Success = true,
                    Message = AuthMessage.LOGIN_SUCCESS,
                    Data = response
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(ResponseCodes.StatusCodes.UNAUTHORIZED, new ApiResponse<LoginResponseDTO>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for email: {Email}", request.Email);
                return StatusCode(ResponseCodes.StatusCodes.INTERNAL_SERVER_ERROR, new ApiResponse<LoginResponseDTO>
                {
                    Success = false,
                    Message = ResponseCodes.Messages.INTERNAL_ERROR,
                    Data = null
                });
            }
        }


        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<RegisterResponseDTO>>> Register([FromBody] RegisterRequestDTO request)
        {
            try
            {
                var response = await _authService.RegisterAsync(request);

                return StatusCode(ResponseCodes.StatusCodes.CREATED, new ApiResponse<RegisterResponseDTO>
                {
                    Success = true,
                    Message = AuthMessage.REGISTER_SUCCESS,
                    Data = response
                });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(ResponseCodes.StatusCodes.CONFLICT, new ApiResponse<RegisterResponseDTO>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration for email: {Email}", request.Email);
                return StatusCode(ResponseCodes.StatusCodes.INTERNAL_SERVER_ERROR, new ApiResponse<RegisterResponseDTO>
                {
                    Success = false,
                    Message = ResponseCodes.Messages.INTERNAL_ERROR,
                    Data = null
                });
            }
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<ApiResponse<LoginResponseDTO>>> RefreshToken([FromBody] RefreshTokenRequestDTO request)
        {
            try
            {
                var response = await _authService.RefreshTokenAsync(request.RefreshToken);

                return StatusCode(ResponseCodes.StatusCodes.OK, new ApiResponse<LoginResponseDTO>
                {
                    Success = true,
                    Message = AuthMessage.REFRESH_SUCCESS,
                    Data = response
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Token refresh failed: {Message}", ex.Message);
                return StatusCode(ResponseCodes.StatusCodes.UNAUTHORIZED, new ApiResponse<LoginResponseDTO>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during token refresh");
                return StatusCode(ResponseCodes.StatusCodes.INTERNAL_SERVER_ERROR, new ApiResponse<LoginResponseDTO>
                {
                    Success = false,
                    Message = ResponseCodes.Messages.INTERNAL_ERROR,
                    Data = null
                });
            }
        }

        [HttpPost("complete-registration")]
        public async Task<ActionResult<ApiResponse<bool>>> CompleteRegistration([FromBody] CompleteRegistrationRequestDTO request)
        {
            try
            {
                var createUserResult = await _authService.CreateUserAfterOTPVerificationAsync(
                    request.Email,
                    request.OTPCode,
                    request.RegistrationToken
                );

                return StatusCode(ResponseCodes.StatusCodes.CREATED, new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Đăng ký thành công! Tài khoản của bạn đã được tạo.",
                    Data = true
                });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(ResponseCodes.StatusCodes.CONFLICT, new ApiResponse<bool>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during completing registration for email: {Email}", request.Email);
                return StatusCode(ResponseCodes.StatusCodes.INTERNAL_SERVER_ERROR, new ApiResponse<bool>
                {
                    Success = false,
                    Message = ResponseCodes.Messages.INTERNAL_ERROR,
                    Data = false
                });
            }
        }
    }
    }
