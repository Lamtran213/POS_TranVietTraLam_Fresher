using POS_TranVietTraLam_Fresher_BLL.DTO.CartItemDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_TranVietTraLam_Fresher_BLL.DTO.CartDTO
{
    public class CartDTO
    {
        public int CartId { get; set; }

        public Guid MemberId { get; set; }

        public DateTime CreatedDate { get; set; }

        public List<CartItemsDTO> CartItems { get; set; } = new();

        public int TotalQuantity => CartItems.Sum(i => i.Quantity);
        public decimal? TotalPrice => CartItems.Sum(i => i.UnitPrice * i.Quantity);
    }
}
