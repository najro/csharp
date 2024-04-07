using BusinessSystem.Extensions;
using BusinessSystem.Models;
using BusinessSystem.Models.Enums;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Graphics.Printing;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Printing;


namespace BusinessSystem
{

    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        Product _selectedProduct;
        Product _selectedBasketProduct;
        Product _selectedStorageProduct;


        
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Product> Products { get; set; }

        public ObservableCollection<Product> FilteredProducts { get; set; }


        public ObservableCollection<Product> BasketProducts { get; set; }


        public void ToggleBasketStatus()
        {
            TextBlockBasketTotal.Text = $"Antal varor: {BasketProducts.Sum(p => p.Reserved)}\nTotalt pris: {BasketProducts.Sum(p => p.Price * p.Reserved)}  kr";
            ButtonBasketClear.IsEnabled = BasketProducts.Count > 0;
            ButtonBasketBuy.IsEnabled = BasketProducts.Count > 0;
        }


        public MainPage()
        {
            this.InitializeComponent();


            Products = new repository.CsvRepository().ReadProductsFromDataFile();

            FilteredProducts = new ObservableCollection<Product>();

            foreach (var product in Products)
            {
                FilteredProducts.Add(product);
            }

            BasketProducts = new ObservableCollection<Product>(Products.Where(p => p.Reserved > 0));

            ToggleBasketStatus();

            this.DataContext = this;

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

            ToggleBasketStatus();
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

            ToggleBasketStatus();
        }

        private void ButtonProductNew_OnClick(object sender, RoutedEventArgs e)
        {
            ButtonProductNew.Visibility = Visibility.Collapsed;
            StackPanelProductEdit.Visibility = Visibility.Visible;
            ButtonProductDelete.Visibility = Visibility.Collapsed;

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

                ToggleBasketStatus();
            }
            else
            {
                var newProduct = GetProductTypeBySelectionName(((ComboBoxItem)ComboBoxProductType.SelectedValue)?.Content.ToString());
                newProduct.Id = Convert.ToInt32(TextBoxProductId.Text);
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
                FilteredProducts.Add(newProduct);
            }




            ButtonProductNew.Visibility = Visibility.Visible;
            StackPanelProductEdit.Visibility = Visibility.Collapsed;

            _selectedStorageProduct = null;
            ListViewStorage.SelectedIndex = -1;


            //UpdateListViewProducts();
            //UpdateListViewStorage();
            //RefreshViews();


        }



        private void ListViewStorage_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedStorageProduct = ((Product)ListViewStorage.SelectedItem);

            // do nothing if no product is selected
            if (_selectedStorageProduct == null)
                return;

            ButtonProductNew.Visibility = Visibility.Collapsed;
            StackPanelProductEdit.Visibility = Visibility.Visible;
            ButtonProductDelete.Visibility = Visibility.Visible;

            EnableTextBoxesByProductAndMode(_selectedStorageProduct, ProductMode.Edit);
            PopulateTextBoxesInputFormByProduct(_selectedStorageProduct);
        }

        private void PopulateTextBoxesInputFormByProduct(Product product)
        {
            if (product == null)
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
                        _selectedStorageProduct = null;
                        ToggleBasketStatus();
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

        private void ButtonBasketClear_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var product in BasketProducts)
            {
                product.Reserved = 0;
            }

            BasketProducts.Clear();
            ToggleBasketStatus();
        }


        private async void ButtonBasketBuy_OnClick(object sender, RoutedEventArgs e)
        {
            if (BasketProducts?.Count == 0)
                return;

            foreach (var product in BasketProducts)
            {
                product.Stock -= product.Reserved;
                product.Reserved = 0;
            }

            // store the basket products in a temporary list to avoid concurrent modification
            var tempBasketProducts = new ObservableCollection<Product>(BasketProducts);

            BasketProducts.Clear();
            ToggleBasketStatus();
        }

        private void CheckValidProductInput()
        {
            ButtonProductSave.IsEnabled = false;

            if (TextBoxProductId.IsValidProductId(TextBoxProductId.Text, Products.ToList(), _selectedStorageProduct) &&
                TextBoxProductName.IsValidProductName(TextBoxProductName.Name) &&
                TextBoxProductPrice.IsValidProductPrice(TextBoxProductPrice.Text) &&
                TextBoxProductStock.IsValidProductStock(TextBoxProductStock.Text) &&
                TextBoxProductPlayTime.IsValidProductPlaytime(TextBoxProductPlayTime.Text))
            {
                ButtonProductSave.IsEnabled = true;
            }
        }

        private void TextBoxProductId_OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            var textInput = textBox?.Text;

            textBox.DisplayValidationColor(textBox.IsValidProductId(textInput, Products.ToList(), _selectedStorageProduct));

            CheckValidProductInput();
        }


        private void TextBoxProductName_OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            var textInput = textBox?.Text;

            textBox.DisplayValidationColor(textBox.IsValidProductName(textInput));

            CheckValidProductInput();
        }

        private void TextBoxProductPrice_OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            var textInput = textBox?.Text;

            textBox.DisplayValidationColor(textBox.IsValidProductPrice(textInput));

            CheckValidProductInput();
        }

        private void TextBoxProductStock_OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            var textInput = textBox?.Text;

            textBox.DisplayValidationColor(textBox.IsValidProductStock(textInput));

            CheckValidProductInput();
        }

        private void TextBoxProductPlayTime_OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            var textInput = textBox?.Text;

            textBox.DisplayValidationColor(textBox.IsValidProductPlaytime(textInput));

            CheckValidProductInput();

        }

        private async void ButtonBasketPrint_OnClick(object sender, RoutedEventArgs e)
        {
            await PrintReceite("Print this magic stuff");
        }


        #region PrintInfo
        // Code borrowed and adjuset from follwing sources:
        // https://stackoverflow.com/questions/36207708/print-multiple-pages-from-a-uwa
        // https://learn.microsoft.com/en-us/windows/uwp/devices-sensors/print-from-your-app


        private PrintDocument printDocument;
        private IPrintDocumentSource printDocumentSource;
        private PrintReceiptPage printPage = null;

        public async Task PrintReceite(string reciete)
        {
            // add recite to print page
            printPage = new PrintReceiptPage();

            //printPage.TextContentBlock() = reciete;
            //printPage.TextContentBlock. = new TextBlock { Text = reciete, FontSize = 24, TextWrapping = TextWrapping.WrapWholeWords };


            printDocument = new PrintDocument();
            printDocumentSource = printDocument.DocumentSource;
            printDocument.GetPreviewPage += GetPrintPreviewPage;

            printDocument.AddPage(printPage);
            printDocument.AddPagesComplete();

            PrintManager printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested += PrintTaskRequested;

            await PrintManager.ShowPrintUIAsync();
        }


        private void GetPrintPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            printDocument.SetPreviewPage(1, printPage);
            printDocument.SetPreviewPageCount(1, PreviewPageCountType.Final);
        }

        void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs e)
        {
            //PrintTask printTask = null;
            //printTask = e.Request.CreatePrintTask("Receipe Print Job", sourceRequested =>
            //{
            //    printTask.Completed += PrintTask_Completed;
            //    sourceRequested.SetSource(printDocumentSource);
            //});
            PrintTask printTask = null;

            // Create a PrintTask and handle print task request
            printTask = e.Request.CreatePrintTask("Receipt Print Job", sourceRequested =>
            {
                // Set the print document source
                sourceRequested.SetSource(printDocumentSource);

                // Handle completed event
                printTask.Completed += (s, args) =>
                {
                    // Check if there were any errors
                    if (args.Completion == PrintTaskCompletion.Failed)
                    {
                        // Handle error
                        Debug.WriteLine("Print task failed.");
                    }
                    else
                    {
                        // Print task completed successfully
                        Debug.WriteLine("Print task completed.");
                    }
                };
            });


        }

        private async void PrintTask_Completed(PrintTask sender, PrintTaskCompletedEventArgs args)
        {


            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                PrintManager printMan = PrintManager.GetForCurrentView();
                printMan.PrintTaskRequested -= PrintTaskRequested;
            });
        }
        #endregion


        private void ButtonProductsSave_OnClick(object sender, RoutedEventArgs e)
        {
            new repository.CsvRepository().WriteProductsToDataFile(Products);
        }
    }

}