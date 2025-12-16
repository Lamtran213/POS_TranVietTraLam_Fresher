using POS_TranVietTraLam_Fresher_Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_TranVietTraLam_Fresher_BLL.DTO.OrderDTO
{
    public class OrderResponseDTO
    {
        public int OrderId { get; set; }
        public Guid MemberId { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidAt { get; set; }
        public decimal TotalAmount { get; set; }
        public string Address { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderDetailResponseDTO> OrderDetailItems { get; set; } = new();
    }
}
