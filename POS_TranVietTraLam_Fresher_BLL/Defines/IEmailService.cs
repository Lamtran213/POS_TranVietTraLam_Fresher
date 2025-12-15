using POS_TranVietTraLam_Fresher_BLL.DTO.CommonDTO;

namespace POS_TranVietTraLam_Fresher_BLL.Defines
{
    public interface IEmailService
    {
        Task<ApiResponse<bool>> SendEmailAsync(string to, string subject, string htmlContent);
    }
}
