using POS_TranVietTraLam_Fresher_BLL.DTO.PaymentDTO;

namespace POS_TranVietTraLam_Fresher_BLL.Defines
{
    public interface IPaymentService
    {
        Task<bool> HandlePayOSWebhook(PayosWebhookPayload payload);
        Task<List<AllPaymentDTO>> GetAllPaymentAsync();
        Task NotifyPaymentChangedAsync();
    }
}
