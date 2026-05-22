using System;

namespace Interview.Web.Models
{
    public class Inventory
    {
        public Guid TransactionId { get; set; } = Guid.NewGuid();
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        
    }

}