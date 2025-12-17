using Microsoft.AspNetCore.Http;

namespace POS_TranVietTraLam_Fresher_BLL.DTO.ProductDTO
{
    public class AddProductRequest
    {
        public int? CategoryId { get; set; }

        public string? ProductName { get; set; }

        public decimal? UnitPrice { get; set; }

        public int UnitsInStock { get; set; }

        public IFormFile? ImageUrl { get; set; }

        public double Discount { get; set; }

        public bool IsActive { get; set; }

        public string? Description { get; set; }
    }
}
