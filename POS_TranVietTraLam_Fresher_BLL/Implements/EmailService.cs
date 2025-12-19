using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using POS_TranVietTraLam_Fresher_BLL.Constants;
using POS_TranVietTraLam_Fresher_BLL.Defines;
using POS_TranVietTraLam_Fresher_BLL.DTO.CommonDTO;
using POS_TranVietTraLam_Fresher_Entities.Entity;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace POS_TranVietTraLam_Fresher_BLL.Implements
{
    public class EmailService : IEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(HttpClient httpClient, IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _httpClient = httpClient;
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> SendEmailAsync(string to, string subject, string htmlContent)
        {
            try
            {
                var requestData = new
                {
                    from = _emailSettings.FromEmail,
                    to = new[] { to },
                    subject = subject,
                    html = htmlContent
                };

                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_emailSettings.ApiKey}");

                var response = await _httpClient.PostAsync("https://api.resend.com/emails", content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Email sent successfully to {to}");
                    return new ApiResponse<bool>
                    {
                        Success = true,
                        Message = EmailMessage.EMAIL_SENT_SUCCESS,
                        Data = true
                    };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed to send email to {to}. Status: {response.StatusCode}, Error: {errorContent}");
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = $"{EmailMessage.EMAIL_SENT_FAILED}: {response.StatusCode}",
                        Data = false
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending email to {to}");
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"{EmailMessage.EMAIL_SENT_FAILED} {ex.Message}",
                    Data = false
                };
            }
        }

        public string BuildPaymentSuccessEmailHtml(
            User user,
            int? orderCode,
            decimal amount,
            DateTime createdAt,
            DateTime paidAt
            )
        {
            // Tìm template trong thư mục project, không phải bin/Debug
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var projectRoot = Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", ".."));
            var templatePath = Path.Combine(projectRoot, "POS_TranVietTraLam_Fresher_BLL", "Templates", "PaymentSuccessEmailTemplate.html");

            // Fallback: tìm trong thư mục hiện tại
            if (!File.Exists(templatePath))
            {
                templatePath = Path.Combine(baseDirectory, "Templates", "PaymentSuccessEmailTemplate.html");
            }

            // Fallback cuối: tìm trong thư mục BLL
            if (!File.Exists(templatePath))
            {
                var bllPath = Path.Combine(projectRoot, "POS_TranVietTraLam_Fresher_BLL", "Templates", "PaymentSuccessEmailTemplate.html");
                if (File.Exists(bllPath))
                {
                    templatePath = bllPath;
                }
            }

            Console.WriteLine($"Looking for template at: {templatePath}");
            Console.WriteLine($"Template exists: {File.Exists(templatePath)}");

            var html = File.ReadAllText(templatePath);

            var vi = new CultureInfo("vi-VN");
            var amountVnd = string.Format(vi, "{0:C0}", amount);

            // Chuyển thời gian UTC sang giờ Việt Nam (UTC+7)
            var createdAtVN = ConvertUtcToVietnamTime(createdAt);
            var paidAtVN = ConvertUtcToVietnamTime(paidAt);

            html = html
                .Replace("{UserName}", $"{(user.Email ?? string.Empty)}".Trim())
                .Replace("{Amount}", amountVnd)
                .Replace("{OrderCode}", orderCode.ToString())
                .Replace("{CreatedAt}", createdAtVN.ToString("HH:mm dd/MM/yyyy"))
                .Replace("{PaidAt}", paidAtVN.ToString("HH:mm dd/MM/yyyy"))
                .Replace("{Year}", DateTime.Now.Year.ToString());

            return html;
        }

        private static DateTime ConvertUtcToVietnamTime(DateTime utcDateTime)
        {
            if (utcDateTime.Kind != DateTimeKind.Utc)
            {
                utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            }

            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, tz);
            }
            catch (TimeZoneNotFoundException)
            {
                try
                {
                    var tz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Bangkok");
                    return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, tz);
                }
                catch
                {
                    return utcDateTime.AddHours(7);
                }
            }
        }
    }
}
