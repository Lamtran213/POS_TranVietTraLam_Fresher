using POS_TranVietTraLam_Fresher_Entities.Enum;

namespace POS_TranVietTraLam_Fresher_BLL.DTO.OrderDTO
{
    public class CreateOrderRequestDTO
    {
        public decimal Freight { get; set; }
        public decimal TotalAmount { get; set; }

        public List<int> CartItemIds { get; set; } = new();

        public string? Address { get; set; }

        /// <summary>
        /// COD = 0, PayOS = 1
        /// </summary>
        public PaymentMethod PaymentMethod { get; set; }
    }
}
