using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentReceived.models
{
    public class OrderTable
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public string Email { get; set; }
        public decimal Price { get; set; }
    }
}
