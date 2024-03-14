using System;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace BusinessSystem.Models
{
    public abstract class Product
    {
        public Product()
        {
        }

        public int Id { get; set; }
        public virtual string Type { get; } = "Product";

        // https://learn.microsoft.com/en-us/windows/uwp/app-resources/images-tailored-for-scale-theme-contrast
        public virtual ImageSource Image { get; set; } = new BitmapImage(new Uri("ms-appx:///Assets/product.png"));

        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; } = 5;

        public int Reserved { get; set; } = 0;

        public virtual string Description
        {
            get
            {
                return ToString();
            }
        }

        public override string ToString()
        {
            return $"{Id}, {Name}, Pris: {Price}, Antal på lager : {Stock}";
        }

        private BitmapImage LoadImage(string filename)
        {
            // https://learn.microsoft.com/en-us/windows/uwp/app-resources/images-tailored-for-scale-theme-contrast
            return new BitmapImage(new Uri("pack://application:,,,/" + filename));
        }
    }
}