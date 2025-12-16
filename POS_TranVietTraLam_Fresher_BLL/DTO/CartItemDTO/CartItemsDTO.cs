using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_TranVietTraLam_Fresher_BLL.DTO.CartItemDTO
{
    public class CartItemsDTO
    {
        public int CartItemId { get; set; }

        public int ProductId { get; set; }

        public string? ProductName { get; set; } = string.Empty;

        public decimal? UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal? Total => UnitPrice * Quantity;

        public string? ImageUrl { get; set; }
    }
}
