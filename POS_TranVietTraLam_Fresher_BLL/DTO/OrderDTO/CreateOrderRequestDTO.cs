using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_TranVietTraLam_Fresher_BLL.DTO.OrderDTO
{
    public class CreateOrderRequestDTO
    {
        public decimal Freight { get; set; }
        public decimal TotalAmount { get; set; }
        public List<int> CartItemIds { get; set; } = new List<int>();
        public string Address { get; set; } = string.Empty;
    }
}
