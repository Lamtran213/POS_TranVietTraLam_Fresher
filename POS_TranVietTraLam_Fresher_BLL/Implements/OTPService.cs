using Microsoft.Extensions.Logging;
using POS_TranVietTraLam_Fresher_BLL.Constants;
using POS_TranVietTraLam_Fresher_BLL.Defines;
using POS_TranVietTraLam_Fresher_BLL.DTO.CommonDTO;
using POS_TranVietTraLam_Fresher_DAL.Defines;
using POS_TranVietTraLam_Fresher_Entities.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_TranVietTraLam_Fresher_BLL.Implements
{
    public class OTPService : IOTPService
    {
        private readonly IOTPRepository _otpRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<OTPService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public OTPService(IOTPRepository otpRepository, IEmailService emailService, ILogger<OTPService> logger, IUnitOfWork unitOfWork)
        {
            _otpRepository = otpRepository;
            _emailService = emailService;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> SendOTPAsync(string email, string purpose)
        {
            try
            {
                var oneHourAgo = DateTime.UtcNow.AddHours(-1);
                var otpCount = await _otpRepository.GetOTPCountByEmailAsync(email, purpose, oneHourAgo);

                if (otpCount >= 5)
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = OtpMessage.OtpSentManyTimes,
                        Data = false
                    };
                }

                await _otpRepository.InvalidateOTPsAsync(email, purpose);

                var otpCode = GenerateOTP();
                var expiresAt = DateTime.UtcNow.AddMinutes(10);

                var otp = new OTP
                {
                    Email = email,
                    OTPCode = otpCode,
                    Purpose = purpose,
                    ExpiresAt = expiresAt,
                    IsUsed = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _otpRepository.AddAsync(otp);
                await _unitOfWork.Save();

                // Gửi email OTP
                var emailSubject = GetOTPEmailSubject(purpose);
                var emailContent = GetOTPEmailContent(email, otpCode, purpose, expiresAt);

                var emailResult = await _emailService.SendEmailAsync(email, emailSubject, emailContent);

                if (!emailResult.Success)
                {
                    _logger.LogError("Failed to send OTP email to {Email}: {Message}", email, emailResult.Message);
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = OtpMessage.FailedToSendOtp,
                        Data = false
                    };
                }

                _logger.LogInformation("OTP sent successfully to {Email} for purpose {Purpose}", email, purpose);

                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = OtpMessage.OtpHasBeenSent,
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending OTP to {Email} for purpose {Purpose}", email, purpose);
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = OtpMessage.OtpHasBeenErrorForSomeReason,
                    Data = false
                };
            }
        }

        private string GenerateOTP()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private string GetOTPEmailSubject(string purpose)
        {
            return purpose switch
            {
                "REGISTER" => "Mã xác thực đăng ký - POS.Lamtran213",
                "RESET_PASSWORD" => "Mã xác thực đặt lại mật khẩu - FitnessCal",
                "CHANGE_EMAIL" => "Mã xác thực thay đổi email - FitnessCal",
                _ => "Mã xác thực - FitnessCal"
            };
        }

        private string GetOTPEmailContent(string email, string otpCode, string purpose, DateTime expiresAt)
        {
            var purposeText = purpose switch
            {
                "REGISTER" => "đăng ký tài khoản",
                "RESET_PASSWORD" => "đặt lại mật khẩu",
                "CHANGE_EMAIL" => "thay đổi email",
                _ => "xác thực"
            };

            // Đọc template HTML
            // Tìm template trong thư mục project, không phải bin/Debug
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var projectRoot = Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", ".."));
            var templatePath = Path.Combine(projectRoot, "POS_TranVietTraLam_Fresher_BLL", "Templates", "OTPEmailTemplate.html");

            // Fallback: tìm trong thư mục hiện tại
            if (!File.Exists(templatePath))
            {
                templatePath = Path.Combine(baseDirectory, "Templates", "OTPEmailTemplate.html");
            }

            // Fallback cuối: tìm trong thư mục BLL
            if (!File.Exists(templatePath))
            {
                var bllPath = Path.Combine(projectRoot, "POS_TranVietTraLam_Fresher_BLL", "Templates", "OTPEmailTemplate.html");
                if (File.Exists(bllPath))
                {
                    templatePath = bllPath;
                }
            }

            Console.WriteLine($"Looking for OTP template at: {templatePath}");
            Console.WriteLine($"OTP template exists: {File.Exists(templatePath)}");

            var htmlTemplate = File.ReadAllText(templatePath);

            // Chuyển thời gian hết hạn từ UTC sang giờ Việt Nam (UTC+7)
            var expiresAtVietnam = ConvertUtcToVietnamTime(expiresAt);

            // Thay thế các placeholder
            var emailContent = htmlTemplate
                .Replace("{PurposeText}", purposeText)
                .Replace("{OTPCode}", otpCode)
                .Replace("{ExpiresAt}", expiresAtVietnam.ToString("HH:mm dd/MM/yyyy"));

            return emailContent;
        }

        private static DateTime ConvertUtcToVietnamTime(DateTime utcDateTime)
        {
            if (utcDateTime.Kind != DateTimeKind.Utc)
            {
                utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            }

            try
            {
                // Windows
                var tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, tz);
            }
            catch (TimeZoneNotFoundException)
            {
                try
                {
                    // Linux/macOS
                    var tz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Bangkok");
                    return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, tz);
                }
                catch
                {
                    // Fallback +7
                    return utcDateTime.AddHours(7);
                }
            }
        }

        public async Task<ApiResponse<bool>> VerifyOTPAsync(string email, string otpCode, string purpose)
        {
            try
            {
                var otp = await _otpRepository.GetValidOTPAsync(email, otpCode, purpose);

                if (otp == null)
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = OtpMessage.OtpInvalid,
                        Data = false
                    };
                }

                // Đánh dấu OTP đã sử dụng
                otp.IsUsed = true;
                otp.UsedAt = DateTime.UtcNow;
                await _unitOfWork.Save();

                _logger.LogInformation("OTP verified successfully for {Email} with purpose {Purpose}", email, purpose);

                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = OtpMessage.OtpVerifiedSuccess,
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying OTP for {Email} with purpose {Purpose}", email, purpose);
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = OtpMessage.OtpHasBeenErrorForSomeReason,
                    Data = false
                };
            }
        }

        public async Task<ApiResponse<bool>> ResendOTPAsync(string email, string purpose)
        {
            return await SendOTPAsync(email, purpose);
        }
    }
}
