using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS_TranVietTraLam_Fresher_Entities.Entity
{
    [Table("CartItem")]
    public partial class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        public int CartId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        [ForeignKey("CartId")]
        [InverseProperty("CartItems")]
        public virtual Cart Cart { get; set; } = null!;

        [ForeignKey("ProductId")]
        [InverseProperty("CartItems")]
        public virtual Product Product { get; set; } = null!;
    }
}
