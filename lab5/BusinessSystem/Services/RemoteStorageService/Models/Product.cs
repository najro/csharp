namespace BusinessSystem.Services.RemoteStorageService.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Genre { get; set; }
        public string Format { get; set; }
        public string Language { get; set; }
        public string Platform { get; set; }
        public int? Playtime { get; set; }
        public string Type { get; set; }
    }
}
