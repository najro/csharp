using System;

namespace BusinessSystem.Models
{
    public class InventoryInfo
    {
        public DateTime DateTime { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
