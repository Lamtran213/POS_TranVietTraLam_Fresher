using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_TranVietTraLam_Fresher_BLL.DTO.PayosDTO
{
    public class PayosDTOs
    {
        public class PayOSPaymentResponse
        {
            public int OrderCode { get; set; }
            public string CheckoutUrl { get; set; } = string.Empty;
            public decimal Amount { get; set; }
            public string Description { get; set; } = string.Empty;
        }

        public class CreatePayOSPaymentRequest
        {
            public int OrderCode { get; set; }
            public decimal Amount { get; set; }
            public string Description { get; set; } = string.Empty;
            public string ReturnUrl { get; set; } = string.Empty;
            public string CancelUrl { get; set; } = string.Empty;
            public List<PayOSItem> Items { get; set; } = new();
        }

        public class PayOSItem
        {
            public string Name { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public decimal Price { get; set; }
            public PayOSItem() { }

            public PayOSItem(string name, int quantity, decimal price)
            {
                Name = name;
                Quantity = quantity;
                Price = price;
            }
        }
    }
}
