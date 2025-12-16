namespace POS_TranVietTraLam_Fresher_BLL.DTO.ProductDTO
{
    public class GetProductResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? CategoryId { get; set; }
        public string ImageUrl { get; set; }
    }
}
