using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS_TranVietTraLam_Fresher_Entities.Entity
{
    [Table("Products")]
    public partial class Product
    {
        [Key]
        public int ProductId { get; set; }

        public int? CategoryId { get; set; }

        [StringLength(40)]
        public string? ProductName { get; set; }

        [Column(TypeName = "money")]
        public decimal? UnitPrice { get; set; }

        public int UnitsInStock { get; set; }

        [StringLength(255)]
        public string? ImageUrl { get; set; }

        public double Discount { get; set; }

        public bool IsActive { get; set; }

        public string? Description { get; set; }

        [InverseProperty("Product")]
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        [ForeignKey("CategoryId")]
        [InverseProperty("Products")]
        public virtual Category? Category { get; set; }

        [InverseProperty("Product")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
