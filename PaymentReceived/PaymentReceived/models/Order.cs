using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentReceived.models
{
    public class Order
    {
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public string Email { get; set; }
        public decimal Price { get; set; }
    }
}
