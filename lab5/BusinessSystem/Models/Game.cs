using System;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;
using Windows.Globalization;

namespace BusinessSystem.Models
{
    public class Game : Product
    {
        public override string Type => "Game";
        public override ImageSource Image { get; set; } = new BitmapImage(new Uri("ms-appx:///Assets/game.png"));

        public string Platform { get; set; }

        public override string ToString()
        {
            return $"Type: {Type}, {Name}, Pris: {Price}, Antal på lager : {Stock}, Reserverade : {Reserved}";
        }

        public override string SearchString()
        {
            return $"${base.SearchString()} plattform:{Platform.ToLower()}";
        }

    }
}
