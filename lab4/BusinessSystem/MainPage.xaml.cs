using System;
using BusinessSystem.Models;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using BusinessSystem.Models.Enums;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI.Core;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.ApplicationModel.Core;


namespace BusinessSystem
{



    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        Models.Product _selectedProduct;
        Models.Product _selectedBasketProduct;
        Models.Product _selectedStorageProduct;

        
        private ObservableCollection<Models.Product> _products;



        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Models.Product> Products { get; set; }

        public ObservableCollection<Models.Product> FilteredProducts { get; set; }


        public ObservableCollection<Models.Product> BasketProducts { get; set; }

        public void SetBasketTotal()
        {
            TextBlockBasketTotal.Text =  $"Antalt produkter {BasketProducts.Sum(p => p.Reserved)}, totalt pris: {BasketProducts.Sum(p => p.Price * p.Reserved)}  kr";
        }


        public MainPage()
        {
            this.InitializeComponent();

            Products = new repository.CsvRepository().ReadProductsFromFile();

            FilteredProducts = new ObservableCollection<Product>();

            foreach (var product in Products)
            {
                FilteredProducts.Add(product);
            }

            BasketProducts = new ObservableCollection<Product>(Products.Where(p => p.Reserved > 0));

            SetBasketTotal();

            this.DataContext = this;

        }



        //private void RefreshViews()
        //{
        //    ValidateProductFromBasket();
        //    ValidateProductToBasket();
        //    UpdateBasketFilter();
        //}

        private void ListViewProducts_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedProduct = ((Models.Product) ListViewProducts.SelectedItem);

            ButtonProductFromBasket.IsEnabled = false;

            ValidateProductToBasket();
        }

        private void ListViewBasket_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedBasketProduct = ((Models.Product) ListViewBasket.SelectedItem);

            ValidateProductFromBasket();
            ValidateProductToBasket();
        }


        private void ValidateProductToBasket()
        {
            ButtonProductToBasket.IsEnabled = _selectedProduct != null;

            if (_selectedProduct == null || _selectedProduct.Stock <= 0 ||
                _selectedProduct.Reserved >= _selectedProduct.Stock)
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

            if (_selectedBasketProduct == null || _selectedBasketProduct.Stock <= 0 ||
                _selectedBasketProduct.Reserved == 0)
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
            
            if (!BasketProducts.Contains(_selectedProduct))
            {
                BasketProducts.Add(_selectedProduct);
            }

            SetBasketTotal();
        }




        private void ButtonProductFromBasket_OnClick(object sender, RoutedEventArgs e)
        {
            _selectedBasketProduct.Reserved -= 1;
            ValidateProductFromBasket();
            ValidateProductToBasket();
          
            if (BasketProducts.Contains(_selectedBasketProduct) && _selectedBasketProduct.Reserved == 0)
            {
                BasketProducts.Remove(_selectedBasketProduct);
            }

            SetBasketTotal();
        }

        private void ButtonProductNew_OnClick(object sender, RoutedEventArgs e)
        {
            ButtonProductNew.Visibility = Visibility.Collapsed;
            StackPanelProductEdit.Visibility = Visibility.Visible;
            SetAllProductTextBoxesToEmpty();
            EnableTextBoxesByProductAndMode(new Product(), ProductMode.New);
        }

        private void ButtonProductSave_OnClick(object sender, RoutedEventArgs e)
        {

            if (_selectedStorageProduct != null)
            {
                _selectedStorageProduct.Name = TextBoxProductName.Text;
                _selectedStorageProduct.Price = Convert.ToDecimal(TextBoxProductPrice.Text);
                _selectedStorageProduct.Stock = Convert.ToInt32(TextBoxProductStock.Text);

                switch (_selectedStorageProduct)
                {
                    case Book book:
                        book.Author = TextBoxProductAuthor.Text;
                        book.Genre = TextBoxProductGenre.Text;
                        book.Format = TextBoxProductFormat.Text;
                        book.Language = TextBoxProductLanguage.Text;
                        break;
                    case Movie movie:
                        movie.Format = TextBoxProductFormat.Text;
                        movie.PlayTime = int.Parse(TextBoxProductPlayTime.Text);
                        break;
                    case Game game:
                        game.Platform = TextBoxProductPlatform.Text;
                        break;
                }
            }
            else
            {
                var newProduct =
                    GetProductTypeBySelectionName(
                        ((ComboBoxItem) ComboBoxProductType.SelectedValue)?.Content.ToString());
                newProduct.Name = TextBoxProductName.Text;
                newProduct.Price = Convert.ToDecimal(TextBoxProductPrice.Text);
                newProduct.Stock = Convert.ToInt32(TextBoxProductStock.Text);

                switch (newProduct)
                {
                    case Book book:
                        book.Author = TextBoxProductAuthor.Text;
                        book.Genre = TextBoxProductGenre.Text;
                        book.Format = TextBoxProductFormat.Text;
                        book.Language = TextBoxProductLanguage.Text;
                        break;
                    case Movie movie:
                        movie.Format = TextBoxProductFormat.Text;
                        movie.PlayTime = int.Parse(TextBoxProductPlayTime.Text);
                        break;
                    case Game game:
                        game.Platform = TextBoxProductPlatform.Text;
                        break;
                }

                Products.Add(newProduct);
            }

            //UpdateListViewProducts();
            //UpdateListViewStorage();
            //RefreshViews();


        }



        private void ListViewStorage_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedStorageProduct = ((Product) ListViewStorage.SelectedItem);

            ButtonProductNew.Visibility = Visibility.Collapsed;
            StackPanelProductEdit.Visibility = Visibility.Visible;

            EnableTextBoxesByProductAndMode(_selectedStorageProduct, ProductMode.Edit);
            PopulateTextBoxesInputFormByProduct(_selectedStorageProduct);
        }

        private void PopulateTextBoxesInputFormByProduct(Product product)
        {
            if(product == null)
                return;

            SetAllProductTextBoxesToEmpty();

            TextBoxProductId.Text = product.Id.ToString();
            TextBoxProductName.Text = product.Name;
            TextBoxProductPrice.Text = product.Price.ToString();
            TextBoxProductStock.Text = product.Stock.ToString();

            switch (product)
            {
                case Book book:
                    //ComboBoxProductType.SelectedIndex = 1;
                    ComboBoxProductType.IsEditable = false;
                    TextBoxProductAuthor.Text = book.Author;
                    TextBoxProductGenre.Text = book.Genre;
                    TextBoxProductFormat.Text = book.Format;
                    TextBoxProductLanguage.Text = book.Language;
                    break;
                case Movie movie:
                    //ComboBoxProductType.SelectedIndex = 2;

                    ComboBoxProductType.IsEditable = false;
                    TextBoxProductFormat.Text = movie.Format;
                    TextBoxProductPlayTime.Text = movie.PlayTime.ToString();
                    break;
                case Game game:
                    //ComboBoxProductType.SelectedIndex = 3;
                    ComboBoxProductType.IsEditable = false;
                    TextBoxProductPlatform.Text = game.Platform;
                    break;
            }

        }




        /// <summary>
        /// Only acctive when a new product is created. Handle the selection of product input based on selected product type
        /// </summary>
        private void ComboBoxProductType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox) sender;
            var selectedValue = (ComboBoxItem) comboBox.SelectedValue;

            var seletedProductType = GetProductTypeBySelectionName(selectedValue?.Content.ToString());


            EnableTextBoxesByProductAndMode(seletedProductType, ProductMode.New);
        }

        private static Product GetProductTypeBySelectionName(string selectedValue)
        {
            var seletedProductType = new Product();

            if (selectedValue == "Bok")
            {
                seletedProductType = new Book();
            }
            else if (selectedValue == "Film")
            {
                seletedProductType = new Movie();
            }
            else if (selectedValue == "Spel")
            {
                seletedProductType = new Game();
            }

            return seletedProductType;
        }

        /// <summary>
        /// Set all textboxes to default empty state
        /// </summary>
        private void SetAllProductTextBoxesToEmpty()
        {
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

        /// <summary>
        /// Set all textboxes to default enabled state based on selected product and mode (edit/new)
        /// </summary>
        private void EnableTextBoxesByProductAndMode(Product product, ProductMode mode)
        {

            // enable selction and id for new product, existing product is not editable
            if (mode == ProductMode.New)
            {
                ComboBoxProductType.IsEnabled = true;
                TextBoxProductId.IsEnabled = true;
            }
            else
            {
                ComboBoxProductType.IsEnabled = false;
                TextBoxProductId.IsEnabled = false;
            }

            // enable all textboxes for new product
            TextBoxProductName.IsEnabled = true;
            TextBoxProductPrice.IsEnabled = true;
            TextBoxProductStock.IsEnabled = true;

            // disable all textboxes
            TextBoxProductAuthor.IsEnabled = false;
            TextBoxProductGenre.IsEnabled = false;
            TextBoxProductFormat.IsEnabled = false;
            TextBoxProductLanguage.IsEnabled = false;
            TextBoxProductPlatform.IsEnabled = false;
            TextBoxProductPlayTime.IsEnabled = false;

            // enable textboxes based on product type
            switch (product)
            {
                case Book book:
                    //ComboBoxProductType.SelectedIndex = 1;
                    TextBoxProductAuthor.IsEnabled = true;
                    TextBoxProductGenre.IsEnabled = true;
                    TextBoxProductFormat.IsEnabled = true;
                    TextBoxProductLanguage.IsEnabled = true;
                    break;

                case Movie movie:
                    //ComboBoxProductType.SelectedIndex = 2;
                    TextBoxProductFormat.IsEnabled = true;
                    TextBoxProductPlayTime.IsEnabled = true;
                    break;

                case Game game:
                    //ComboBoxProductType.SelectedIndex = 3;
                    TextBoxProductPlatform.IsEnabled = true;
                    break;
            }


        }

        private void ButtonProductCancel_OnClick(object sender, RoutedEventArgs e)
        {
            _selectedStorageProduct = null;

            ButtonProductNew.Visibility = Visibility.Visible;
            StackPanelProductEdit.Visibility = Visibility.Collapsed;

        }

        private async void ButtonProductDelete_OnClick(object sender, RoutedEventArgs e)
        {
            if (_selectedStorageProduct != null)
            {
                if (_selectedStorageProduct.Stock == 0)
                {
                    RemoveSelectedProductFromAllLists(_selectedStorageProduct);
                }
                else
                {
                    var dialog = new ContentDialog
                    {
                        Title = "Ta bort produkt",
                        Content = "Det finns flera produkter på lager, är du säker på att du vill ta bort den?",
                        PrimaryButtonText = "Ja",
                        SecondaryButtonText = "Nej",
                        CloseButtonText = "Avbryt"
                    };

                    dialog.PrimaryButtonClick += async (s, args) =>
                    {
                        RemoveSelectedProductFromAllLists(_selectedStorageProduct);
                        SetBasketTotal();
                    };

                    dialog.SecondaryButtonClick += (s, args) =>
                    {
                      // do nothing
                    };

                    await dialog.ShowAsync();

                }
               
            }
        }

        private void RemoveSelectedProductFromAllLists(Product product)
        {
            if (BasketProducts.Contains(product))
            {
                BasketProducts.Remove(product);
            }

            if (Products.Contains(product))
            {
                Products.Remove(product);
            }

            if (FilteredProducts.Contains(product))
            {
                FilteredProducts.Remove(product);
            }
        }

     

        private void TextBoxSearch_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = TextBoxSearch.Text.ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                FilteredProducts.Clear();
                foreach (var product in Products)
                {
                    FilteredProducts.Add(product);
                }
            }
            else
            {
                var filteredProducts = Products.Where(p => p.SearchString().Contains(searchText));

                FilteredProducts.Clear();

                foreach (var product in filteredProducts)
                {
                    FilteredProducts.Add(product);
                }

            }
        }

        private void TextBoxProductId_OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            var textInput = textBox?.Text;


            // verify that input is valid
            if (!int.TryParse(textInput, out int inputValue) || (inputValue < 0 || inputValue >= int.MaxValue))
            {
                textBox.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
            }
            else
            {
                // valid value
                textBox.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Green);
            }

            CheckValidProductInput();
        }

        private void CheckValidProductInput()
        {
            ButtonProductSave.IsEnabled = true;
        }

        private void ButtonBasketBuy_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}