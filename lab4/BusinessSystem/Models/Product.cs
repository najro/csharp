﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace BusinessSystem.Models
{
    public class Product  : INotifyPropertyChanged
    {
        public Product()
        {
            Stock = 5;
            Reserved = 0;
        }

        public int Id { get; set; }
        public virtual string Type { get; } = "Product";

        // https://learn.microsoft.com/en-us/windows/uwp/app-resources/images-tailored-for-scale-theme-contrast
        public virtual ImageSource Image { get; set; } = new BitmapImage(new Uri("ms-appx:///Assets/product.png"));

        public string Name { get; set; }

      

        decimal _price = 0;
        public decimal Price
        {
            get
            {
                return _price;
            }
            set
            {
                _price = value;
                OnNotifyPropertyChanged();

                Description = ToString();
            }
        }


        int _stock = 0;
        public int Stock
        {
            get
            {
                return _stock;
            }
            set
            {
                _stock = value;
                OnNotifyPropertyChanged();

                Description = ToString();
            }
        }

        

        int _reserved = 0;
        public int Reserved
        {
            get
            {
                return _reserved;
            }
            set
            {
                _reserved = value;
                OnNotifyPropertyChanged();

                Description = ToString();
            }
        }


        string _description = "";

        public virtual string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                OnNotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnNotifyPropertyChanged([CallerMemberName] string memberName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(memberName));
            }
        }

        public override string ToString()
        {
            return $"{Id}, {Name}, Pris: {Price}, Antal på lager : {Stock}, Reserverade : {Reserved}";
        }

        private BitmapImage LoadImage(string filename)
        {
            // https://learn.microsoft.com/en-us/windows/uwp/app-resources/images-tailored-for-scale-theme-contrast
            return new BitmapImage(new Uri("pack://application:,,,/" + filename));
        }
    }
}