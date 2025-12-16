using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_TranVietTraLam_Fresher_BLL.DTO.ProductDTO
{
    public class AddProductRequest
    {
        public int? CategoryId { get; set; }

        public string? ProductName { get; set; }

        public decimal? UnitPrice { get; set; }

        public int UnitsInStock { get; set; }

        [StringLength(255)]
        public string? ImageUrl { get; set; }

        public double Discount { get; set; }

        public bool IsActive { get; set; }

        public string? Description { get; set; }
    }
}
