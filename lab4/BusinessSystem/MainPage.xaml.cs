using BusinessSystem.Models;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BusinessSystem
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ObservableCollection<Models.Product> products = new ObservableCollection<Models.Product>();
        Models.Product _selectedProduct;

        public ICollectionView FilteredViewProducts { get; private set; }
        public ICollectionView FilteredViewBasket { get; private set; }


        public MainPage()
        {
            this.InitializeComponent();



            products = new repository.CsvRepository().ReadProductsFromFile();


            FilteredViewProducts = new CollectionViewSource
            {
                Source = products
            }.View;

            

            FilteredViewBasket = new CollectionViewSource
            {
                Source = products.Where(p => ((Product)p).Reserved > 0)
            }.View;
            


            ListViewProducts.ItemsSource = FilteredViewProducts;
            ListViewBasket.ItemsSource = FilteredViewBasket;

            this.DataContext = this;
        }

        private void RefreshViews()
        {
            //FilteredViewProducts.GetDefaultView(Products).Refresh();
            // FilteredViewProducts
            ListViewProducts.UpdateLayout();
        }

        private void ListViewProducts_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedProduct = ((Models.Product)ListViewProducts.SelectedItem);

            ValidateProductFromBasket();
            ValidateProductToBasket();
        }

        private void ValidateProductToBasket()
        {
            ButtonProductToBasket.IsEnabled = _selectedProduct != null;

            if (_selectedProduct == null || _selectedProduct.Stock <= 0 || _selectedProduct.Reserved >= _selectedProduct.Stock)
            {
                ButtonProductToBasket.IsEnabled = false;
            }
            else
            {
                ButtonProductToBasket.IsEnabled = true;
            }
        }

        private void ValidateProductFromBasket()
        {
            ButtonProductFromBasket.IsEnabled = _selectedProduct != null;

            if (_selectedProduct == null || _selectedProduct.Stock <= 0 || _selectedProduct.Reserved >= _selectedProduct.Stock)
            {
                ButtonProductToBasket.IsEnabled = false;
            }
            else
            {
                ButtonProductToBasket.IsEnabled = true;
            }
        }

        private void ButtonProductToBasket_OnClick(object sender, RoutedEventArgs e)
        {
            _selectedProduct.Reserved += 1;
            ValidateProductFromBasket();
            ValidateProductToBasket();
            RefreshViews();
        }

        private void ButtonProductFromBasket_OnClick(object sender, RoutedEventArgs e)
        {
            _selectedProduct.Reserved -= 1;
            ValidateProductFromBasket();
            ValidateProductToBasket();
            RefreshViews();
        }
    }
}
