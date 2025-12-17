using POS_TranVietTraLam_Fresher_Entities.Enum;

namespace POS_TranVietTraLam_Fresher_BLL.DTO.PaymentDTO
{
    public class AllPaymentDTO
    {
        public int PaymentId { get; set; }

        public int OrderId { get; set; }
        public string Email { get; set; } = string.Empty;

        public decimal Amount { get; set; }
        public int? PayosOrderCode { get; set; }

        public PaymentStatus Status { get; set; }
        public PaymentMethod Method { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
