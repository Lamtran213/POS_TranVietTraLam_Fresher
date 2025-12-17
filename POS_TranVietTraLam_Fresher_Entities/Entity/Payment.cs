using POS_TranVietTraLam_Fresher_Entities.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS_TranVietTraLam_Fresher_Entities.Entity
{
    [Table("Payments")]
    public class Payment
    {
        public int PaymentId { get; set; }

        public int OrderId { get; set; }
        public Guid UserId { get; set; }

        public decimal Amount { get; set; }
        public int? PayosOrderCode { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public PaymentMethod Method { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }

        public User User { get; set; } = null!;
        public Order Order { get; set; } = null!;
    }
}
