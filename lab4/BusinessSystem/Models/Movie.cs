﻿using System;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;
using Windows.Foundation.Metadata;

namespace BusinessSystem.Models
{
    public class Movie : Product
    {
        public override string Type => "Movie";
        public override ImageSource Image { get; set; } = new BitmapImage(new Uri("ms-appx:///Assets/movie.png"));

        public string Format { get; set; }
        public int PlayTime { get; set; }

        public override string ToString()
        {
            return $"Type: {Type}, {Name}, Pris: {Price}, Antal på lager : {Stock}, Reserverade : {Reserved}";
        }

        public override string SearchString()
        {
            return $"${base.SearchString()} format:{Format.ToLower()} speltid:{PlayTime}";
        }
    }
}
