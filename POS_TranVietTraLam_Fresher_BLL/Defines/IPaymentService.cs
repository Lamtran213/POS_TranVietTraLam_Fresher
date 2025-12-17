using POS_TranVietTraLam_Fresher_BLL.DTO.PaymentDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_TranVietTraLam_Fresher_BLL.Defines
{
    public interface IPaymentService
    {
        Task<bool> HandlePayOSWebhook(PayosWebhookPayload payload);
    }
}
