using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS_TranVietTraLam_Fresher_Entities.Entity
{
    [Table("Cart")]
    public partial class Cart
    {
        [Key]
        public int CartId { get; set; }

        public Guid UserId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedDate { get; set; }

        [InverseProperty("Cart")]
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        [ForeignKey("UserId")]
        [InverseProperty("Carts")]
        public virtual User User { get; set; } = null!;
    }
}
