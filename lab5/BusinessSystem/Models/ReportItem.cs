namespace BusinessSystem.Models
{
    public class ReportItem
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal TotalSales { get; internal set; }
    }
}