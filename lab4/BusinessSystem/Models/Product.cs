using BusinessSystem.Extensions;
using System;
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
            Available = CalculateAvailable();
        }

        public int Id { get; set; }
        public virtual string Type { get; } = "Product";

        // https://learn.microsoft.com/en-us/windows/uwp/app-resources/images-tailored-for-scale-theme-contrast
        public virtual ImageSource Image { get; set; } = new BitmapImage(new Uri("ms-appx:///Assets/product.png"));

        
        string _name = "";
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnNotifyPropertyChanged();
            }
        }

        int _price = 0;
        public int Price
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

                Available = CalculateAvailable();
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

                Available = CalculateAvailable();
                Description = ToString();
            }
        }

        int _available = 0;
        public int Available
        {
            get
            {
                return _available;
            }
            set
            {
                _available = value;
                OnNotifyPropertyChanged();
                Description = ToString();
            }
        }

        private int CalculateAvailable()
        {
            return Stock - Reserved;
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



        // https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/how-to-implement-property-change-notification?view=netframeworkdesktop-4.8
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnNotifyPropertyChanged([CallerMemberName] string memberName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(memberName));
            }
        }

        public virtual string SearchString()
        {
            return $"id:{Id} typ:{this.GetTypeNameTranslation().ToLower()} namn:{Name.ToLower()} pris:{Price} antal:{Stock}";
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