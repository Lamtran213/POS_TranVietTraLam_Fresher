using POS_TranVietTraLam_Fresher_BLL.DTO.CommonDTO;
using POS_TranVietTraLam_Fresher_Entities.Entity;

namespace POS_TranVietTraLam_Fresher_BLL.Defines
{
    public interface IEmailService
    {
        Task<ApiResponse<bool>> SendEmailAsync(string to, string subject, string htmlContent);
        string BuildPaymentSuccessEmailHtml(
            User user,
            int? orderCode,
            decimal amount,
            DateTime createdAt,
            DateTime paidAt
            );
    }
}
