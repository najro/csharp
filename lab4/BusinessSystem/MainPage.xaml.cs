using System;
using BusinessSystem.Models;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Storage.Streams;
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
        Models.Product _selectedProduct;
        Models.Product _selectedBasketProduct;
        Models.Product _selectedStorageProduct;

        public ICollectionView FilteredViewProducts { get; private set; }
        public ICollectionView FilteredViewBasket { get; private set; }


        public CollectionViewSource FilteredViewSource { get; set; }

        //// Apply a filter to the CollectionViewSource
        //FilteredViewSource.View.Filter = (item) =>
        //{
        //    // Implement your filter logic here
        //    // For example, filtering based on price
        //    // This example filters out products with a price less than 10
        //    if (item is Product product)
        //    {
        //        return product.Price >= 10;
        //    }
        //    return false;
        //};


ObservableCollection<Models.Product> _products = new ObservableCollection<Models.Product>();
        public ObservableCollection<Models.Product> Products
        {
            get
            {
                return _products;
            }
            set
            {
                if (_products != value)
                {
                    _products = value;
                }
            }
        }

   

        private void UpdateBasketFilter()
        {
             var filteredProducts = Products.Where(p => p.Reserved > 0).ToList();
            FilteredViewSource.Source = new ObservableCollection<Product>(filteredProducts);
            //ListViewBasket.ItemsSource = FilteredViewSource.View;

            ListViewBasket.ItemsSource = null;
            ListViewBasket.ItemsSource = FilteredViewSource.View;

            ListViewBasket.SelectedItem = null;
        }

        public MainPage()
        {
            this.InitializeComponent();

            Products = new repository.CsvRepository().ReadProductsFromFile();

            FilteredViewSource = new CollectionViewSource();
            UpdateBasketFilter();
           
        }

        private void RefreshViews()
        {
            ValidateProductFromBasket();
            ValidateProductToBasket();
            UpdateBasketFilter();
        }

        private void ListViewProducts_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedProduct = ((Models.Product)ListViewProducts.SelectedItem);

            ButtonProductFromBasket.IsEnabled = false;
            
            ValidateProductToBasket();
        }

        private void ListViewBasket_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedBasketProduct = ((Models.Product)ListViewBasket.SelectedItem);

            ValidateProductFromBasket();
            ValidateProductToBasket();
        }


        private void ListViewStorage_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedStorageProduct = ((Models.Product)ListViewStorage.SelectedItem);


            TextBoxProductId.IsEnabled = false;
            ComboBoxProductType.IsEnabled = false;
            TextBoxProductId.Text = _selectedStorageProduct.Id.ToString();
            TextBoxProductName.Text = _selectedStorageProduct.Name;
            TextBoxProductPrice.Text = _selectedStorageProduct.Price.ToString();
            TextBoxProductStock.Text = _selectedStorageProduct.Stock.ToString();

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
            ButtonProductFromBasket.IsEnabled = _selectedBasketProduct != null;

            if (_selectedBasketProduct == null || _selectedBasketProduct.Stock <= 0 || _selectedBasketProduct.Reserved == 0 )
            {
                ButtonProductFromBasket.IsEnabled = false;
            }
            else
            {
                ButtonProductFromBasket.IsEnabled = true;
            }
        }

        private void ButtonProductToBasket_OnClick(object sender, RoutedEventArgs e)
        {
            _selectedProduct.Reserved += 1;
            ValidateProductFromBasket();
            ValidateProductToBasket();
            RefreshViews();

            //Products.Remove(_selectedProduct);
        }

        private void ButtonProductFromBasket_OnClick(object sender, RoutedEventArgs e)
        {
            _selectedProduct.Reserved -= 1;
            ValidateProductFromBasket();
            ValidateProductToBasket();
            RefreshViews();
        }

        private void ButtonProductNew_OnClick(object sender, RoutedEventArgs e)
        {
            ButtonProductNew.Visibility = Visibility.Collapsed;
            StackPanelProductEdit.Visibility = Visibility.Visible;
            SetAllProductTextBoxesToEmpty();
            SetAllProductTextBoxesDefaultEnabled();
        }

        private void ButtonProductSave_OnClick(object sender, RoutedEventArgs e)
        {
            Products.Add(new Models.Product { Name = TextBoxProductName.Text, Price = 100, Stock = 5 });
            
        }

        // only active when a new product is created
        private void ComboBoxProductType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            var selectedValue = (ComboBoxItem)comboBox.SelectedValue;

            SetAllProductTextBoxesDefaultEnabled(selectedValue?.Content.ToString());
        }

        
        private void SetAllProductTextBoxesToEmpty()
        {
            // clear all textboxes
            TextBoxProductId.Text = "";
            TextBoxProductName.Text = "";
            TextBoxProductPrice.Text = "";
            TextBoxProductStock.Text = "";
            TextBoxProductAuthor.Text = "";
            TextBoxProductGenre.Text = "";
            TextBoxProductFormat.Text = "";
            TextBoxProductLanguage.Text = "";
            TextBoxProductPlatform.Text = "";
            TextBoxProductPlayTime.Text = "";
        }

        private void SetAllProductTextBoxesDefaultEnabled(string selectedProductType = "")
        {
            // enable/disable all textboxes 
            ComboBoxProductType.Visibility = Visibility.Visible;
            ComboBoxProductType.IsEnabled = true;
            TextBoxProductId.IsEnabled = true;
            TextBoxProductName.IsEnabled = true;
            TextBoxProductPrice.IsEnabled = true;
            TextBoxProductStock.IsEnabled = true;

            TextBoxProductAuthor.IsEnabled = false;
            TextBoxProductGenre.IsEnabled = false;
            TextBoxProductFormat.IsEnabled = false;
            TextBoxProductLanguage.IsEnabled = false;
            TextBoxProductPlatform.IsEnabled = false;
            TextBoxProductPlayTime.IsEnabled = false;

            switch (selectedProductType)
            {
                case "Bok":
                    TextBoxProductAuthor.IsEnabled = true;
                    TextBoxProductGenre.IsEnabled = true;
                    TextBoxProductFormat.IsEnabled = true;
                    TextBoxProductLanguage.IsEnabled = true;
                    break;

                case "Film":
                    TextBoxProductFormat.IsEnabled = true;
                    TextBoxProductPlayTime.IsEnabled = true;
                    break;

                case "Musik":
                    TextBoxProductPlatform.IsEnabled = true;
                    break;
            }
        }
    }
}
