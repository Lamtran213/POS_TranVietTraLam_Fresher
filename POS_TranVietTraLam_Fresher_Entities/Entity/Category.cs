using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS_TranVietTraLam_Fresher_Entities.Entity
{
    [Table("Categories")]
    public partial class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [StringLength(40)]
        public string? CategoryName { get; set; }

        public string Description { get; set; } = null!;

        [InverseProperty("Category")]
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
