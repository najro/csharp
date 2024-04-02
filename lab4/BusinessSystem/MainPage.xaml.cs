using BusinessSystem.Models;
using BusinessSystem.Models.Enums;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


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

        //public MainPageViewModel ViewModel { get; set; }


        private ObservableCollection<Models.Product> _products;



        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Models.Product> Products { get; set; }

        public ObservableCollection<Models.Product> BasketProducts { get; set; }





        public MainPage()
        {
            this.InitializeComponent();

            Products = new repository.CsvRepository().ReadProductsFromFile();

            BasketProducts = new ObservableCollection<Product>(Products.Where(p => p.Reserved > 0));



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

            if (_selectedBasketProduct == null || _selectedBasketProduct.Stock <= 0 || _selectedBasketProduct.Reserved == 0)
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
            //RefreshViews();


            //add _selectedProduct to BasketProducts if not exist
            if (!BasketProducts.Contains(_selectedProduct))
            {
                BasketProducts.Add(_selectedProduct);
            }
            //else
            //{
            //    _selectedProduct.Reserved += 1;
            //    //BasketProducts.Remove(_selectedProduct);
            //    //BasketProducts.Add(_selectedProduct);
            //}
        }




        private void ButtonProductFromBasket_OnClick(object sender, RoutedEventArgs e)
        {
            _selectedBasketProduct.Reserved -= 1;
            ValidateProductFromBasket();
            ValidateProductToBasket();
            // RefreshViews();
            //remove _selectedProduct from BasketProducts if exist
            if (BasketProducts.Contains(_selectedBasketProduct) && _selectedBasketProduct.Reserved == 0)
            {
                BasketProducts.Remove(_selectedBasketProduct);
            }
            //else
            //{
            //    _selectedBasketProduct.Reserved -= 1;
            //     BasketProducts.Remove(_selectedBasketProduct);
            //     BasketProducts.Add(_selectedBasketProduct);
            // }
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
                        movie.PlayTime = TextBoxProductPlayTime.Text;
                        break;
                    case Game game:
                        game.Platform = TextBoxProductPlatform.Text;
                        break;
                }
            }
            else
            {
                var newProduct = GetProductTypeBySelectionName(((ComboBoxItem)ComboBoxProductType.SelectedValue)?.Content.ToString());
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
                        movie.PlayTime = TextBoxProductPlayTime.Text;
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
            _selectedStorageProduct = ((Product)ListViewStorage.SelectedItem);

            ButtonProductNew.Visibility = Visibility.Collapsed;
            StackPanelProductEdit.Visibility = Visibility.Visible;

            EnableTextBoxesByProductAndMode(_selectedStorageProduct, ProductMode.Edit);
            PopulateTextBoxesInputFormByProduct(_selectedStorageProduct);
        }

        private void PopulateTextBoxesInputFormByProduct(Product product)
        {
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
                    TextBoxProductPlayTime.Text = movie.PlayTime;
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
            var comboBox = (ComboBox)sender;
            var selectedValue = (ComboBoxItem)comboBox.SelectedValue;

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

        private void ButtonProductDelete_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

    }
}
