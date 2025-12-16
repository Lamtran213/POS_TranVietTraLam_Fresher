namespace POS_TranVietTraLam_Fresher_BLL.DTO.OrderDTO
{
    public class OrderDetailResponseDTO
    {
        public int OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public double Discount { get; set; }
        public string? ProductName { get; set; }
        public string? ImageUrl { get; set; }
        public decimal SubTotalPrice => UnitPrice * Quantity;
        public decimal TotalPrice => (UnitPrice * Quantity * (decimal)(1 - Discount));
    }
}
