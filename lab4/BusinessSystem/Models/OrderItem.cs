using System;

namespace BusinessSystem.Models
{
    public class OrderItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public double Price { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
