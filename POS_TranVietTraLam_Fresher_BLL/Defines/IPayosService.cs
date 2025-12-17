using static POS_TranVietTraLam_Fresher_BLL.DTO.PayosDTO.PayosDTOs;

namespace POS_TranVietTraLam_Fresher_BLL.Defines
{
    public interface IPayosService
    {
        Task<PayOSPaymentResponse> CreatePaymentLinkAsync(CreatePayOSPaymentRequest request);
        Task<bool> CancelPaymentLinkAsync(int orderCode, string? cancellationReason = null);
    }
}
