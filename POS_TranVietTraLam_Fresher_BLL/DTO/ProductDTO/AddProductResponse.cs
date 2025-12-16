using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_TranVietTraLam_Fresher_BLL.DTO.ProductDTO
{
    public class AddProductResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; }
        public decimal? UnitPrice { get; set; }
        public int UnitsInStock { get; set; }
        public int? CategoryId { get; set; }
        public string ImageUrl { get; set; }
        public double Discount { get; set; }

        public bool IsActive { get; set; }
    }
}
