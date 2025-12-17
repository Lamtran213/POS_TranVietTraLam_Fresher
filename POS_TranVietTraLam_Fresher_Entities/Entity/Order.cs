using POS_TranVietTraLam_Fresher_Entities.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS_TranVietTraLam_Fresher_Entities.Entity
{
    [Table("Orders")]
    public partial class Order
    {
        [Key]
        public int OrderId { get; set; }

        public Guid UserId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Column(TypeName = "datetime")]
        public DateTime? RequiredDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ShippedDate { get; set; }

        [Column(TypeName = "money")]
        public decimal? Freight { get; set; }

        public bool IsPaid { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? PaidAt { get; set; }

        [Column(TypeName = "money")]
        public decimal TotalAmount { get; set; }

        public OrderStatus OrderStatus { get; set; }

        [StringLength(4000)]
        public string? Address { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("Orders")]
        public virtual User Users { get; set; } = null!;
        [InverseProperty("Order")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
