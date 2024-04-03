using System;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;

namespace BusinessSystem.Models
{
    public class Book : Product
    {
        public override string Type => "Book";
        public override ImageSource Image { get; set; } = new BitmapImage(new Uri("ms-appx:///Assets/book.png"));

        public string Author { get; set; }
        public string Genre { get; set; }
        public string Format { get; set; }
        public string Language { get; set; }
        public override string ToString()
        {
            return $"Type: {Type}, {Name}, Pris: {Price}, Antal på lager : {Stock}, Reserverade : {Reserved}";
        }

        public override string SearchString()
        {
            return $"${base.SearchString()} forfattare:{Author.ToLower()} schanger:{Genre.ToLower()} format:{Format.ToLower()} språk:{Language.ToLower()}";
        }
    }
}
