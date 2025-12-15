using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS_TranVietTraLam_Fresher_Entities.Entity
{
    [Table("OrderDetail")]
    public partial class OrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }

        [Column(TypeName = "money")]
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public double Discount { get; set; }

        [ForeignKey("OrderId")]
        [InverseProperty("OrderDetails")]
        public virtual Order Order { get; set; } = null!;

        [ForeignKey("ProductId")]
        [InverseProperty("OrderDetails")]
        public virtual Product Product { get; set; } = null!;
    }
}
