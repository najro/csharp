﻿using BusinessSystem.Extensions;
using BusinessSystem.Helpers;
using BusinessSystem.Models;
using BusinessSystem.Models.Constants;
using BusinessSystem.Models.Enums;
using BusinessSystem.Repositories;
using BusinessSystem.Services.RemoteStorageService;
using BusinessSystem.Services.RemoteStorageService.Models;
using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Printing;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Printing;
using static BusinessSystem.Models.Constants.Constants;
using Product = BusinessSystem.Models.Product;


namespace BusinessSystem
{
    /// <summary>
    /// This is the main page of the business system app
    /// Backend code :  (lab5 - alfoande100 / Örjan Andersson)
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Product _selectedProduct;
        Product _selectedBasketProduct;
        Product _selectedStorageProduct;
        Product _selectedInventoryProduct;

        // storage for the inventory list
        public ObservableCollection<InventoryInfo> InventoryList { get; set; }

        // storage for the products that is available in total
        public ObservableCollection<Product> Products { get; set; }
        
        // storage for the products that is filtered thas is visible for the user to buy in shop
        public ObservableCollection<Product> FilteredProducts { get; set; }

        // storage for the products that is currently in the basket
        public ObservableCollection<Product> BasketProducts { get; set; }

        // timer for updating the products from remote storage
        DispatcherTimer _timerUpdateProducts;

        public MainPage()
        {
            this.InitializeComponent();

            // read all products from file
            Products = new ObservableCollection<Product>(new Repositories.ProductsRepository().GetProducts());

            // set the filtered products for user to buy
            FilteredProducts = new ObservableCollection<Product>();

            // set the inventory list to keep track of the stock for each product
            InventoryList = new ObservableCollection<InventoryInfo>();

            foreach (var product in Products)
            {
                FilteredProducts.Add(product);
            }

            // set the basket products
            BasketProducts = new ObservableCollection<Product>(Products.Where(p => p.Reserved > 0));

            // set the basket status
            ToggleBasketStatus();

            // set info in pivot for info about data files
            TextBoxDataFiles.Text = $"Datafiler blir lagrade här:\n{ApplicationData.Current.LocalFolder.Path}";

            // update the local products from remote storage during startup
            UpdateLocalProductsFromRemoteStorage();

            // start the timer for updating the local products from remote storage
            SetupTimerForProductsUpdate();

            // set the historic type combobox to default display info for number of products in stock
            ComboBoxHistoricType.SelectedIndex = 0;

            this.DataContext = this;
        }

        #region Time Update Products from Remote Storage
        /// <summary>
        /// Update the local products from remote storage each minute
        /// </summary>
        public void SetupTimerForProductsUpdate()
        {
            // https://learn.microsoft.com/en-us/uwp/api/windows.ui.xaml.dispatchertimer?view=winrt-22621
            // setup a timer for updating the products from remote storage each minute

            _timerUpdateProducts = new DispatcherTimer();
            _timerUpdateProducts.Interval = new TimeSpan(0, 1, 0);

            _timerUpdateProducts.Tick += async (s, e) =>
            {
                UpdateLocalProductsFromRemoteStorage();
            };

            _timerUpdateProducts.Start();
        }
        #endregion

        #region Pivot Butik

        /// <summary>
        /// toggle the basket status
        /// </summary>
        public void ToggleBasketStatus()
        {
            TextBlockBasketTotal.Text = $"Antal varor: {BasketProducts.Sum(p => p.Reserved)}\nTotalt pris: {BasketProducts.Sum(p => p.Price * p.Reserved)}  kr";
            ButtonBasketClear.IsEnabled = BasketProducts.Count > 0;
            ButtonBasketBuy.IsEnabled = BasketProducts.Count > 0;
        }

        /// <summary>
        /// Event handler when the casheer selects a product in the listview of aviailable products in store
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewProducts_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedProduct = ((Models.Product)ListViewProducts.SelectedItem);

            ButtonProductFromBasket.IsEnabled = false;

            ValidateProductToBasket();
        }

        /// <summary>
        /// Event handler when the casheer selects a product in the listview of products in the basket
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewBasket_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedBasketProduct = ((Models.Product)ListViewBasket.SelectedItem);

            ValidateProductFromBasket();
            ValidateProductToBasket();
        }


        /// <summary>
        /// Validate the button for adding a product to the basket
        /// </summary>
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


        /// <summary>
        /// Validate the button for removing a product from the basket
        /// </summary>
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

        /// <summary>
        /// Add a product to the basket
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Remove a product from the basket
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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


        /// <summary>
        /// Buy the products in the basket
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonBasketBuy_OnClick(object sender, RoutedEventArgs e)
        {
            if (BasketProducts?.Count == 0)
                return;

            // https://docs.microsoft.com/en-us/windows/uwp/design/controls-and-patterns/dialogs-and-flyouts/dialogs
            var dialog = new ContentDialog
            {
                Title = "Köp",
                Content = "Är kunden klar och skall betala sina varor",
                PrimaryButtonText = "Ja",
                SecondaryButtonText = "Nej",
                CloseButtonText = "Avbryt"
            };

            dialog.PrimaryButtonClick += async (s, args) =>
            {


                // Build reciete
                lastestReceiptInfo = BuildRecieptPrint(BasketProducts.ToList());
                ButtonBasketPrint.IsEnabled = true;


                /// Save the order to file
                var orderList = new List<OrderItem>();
                orderList = OrderItemHelper.BuildOrderItemsFromProducts(BasketProducts.ToList(), Guid.NewGuid(), DateTime.Now);

                new Repositories.OrderRepository().WriteOrderItemsToDataFile(orderList);

                // NOTE
                // Skip this because it is more efficient to update the stock only for the products that is in the basket
                // If you should follow specification this should be done. The spec also says, do it efficient.
                //SyncLocalProductsToRemoteStorage(Products.ToList());

                // Clear the basket and update the stock
                foreach (var product in BasketProducts)
                {
                    product.Stock -= product.Reserved;

                    // if any product get negative stock due update from local storage just before the buy
                    // the prodocts is still in the basket/store and the stock is set to 0
                    if (product.Stock < 0) 
                    {
                        product.Stock = 0;
                    }

                    product.Reserved = 0;
                }

                SyncLocalProductsToRemoteStorage(BasketProducts.ToList());

                BasketProducts.Clear();
                ToggleBasketStatus();
            };

            dialog.SecondaryButtonClick += (s, args) =>
            {
                // do nothing
            };

            await dialog.ShowAsync();
        }

        /// <summary>
        /// Event handler for the search text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Event handler for the clear basket button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonBasketClear_OnClick(object sender, RoutedEventArgs e)
        {

            // https://docs.microsoft.com/en-us/windows/uwp/design/controls-and-patterns/dialogs-and-flyouts/dialogs
            var dialog = new ContentDialog
            {
                Title = "Töm korg",
                Content = "Önskar du tömma alla produkter i korgen?",
                PrimaryButtonText = "Ja",
                SecondaryButtonText = "Nej",
                CloseButtonText = "Avbryt"
            };

            dialog.PrimaryButtonClick += async (s, args) =>
            {
                foreach (var product in BasketProducts)
                {
                    product.Reserved = 0;
                }

                BasketProducts.Clear();
                ToggleBasketStatus();
            };

            dialog.SecondaryButtonClick += (s, args) =>
            {
                // do nothing
            };

            await dialog.ShowAsync();

        }


        #endregion

        #region Pivot Lager

        /// <summary>
        /// Add a new product to the storage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonProductNew_OnClick(object sender, RoutedEventArgs e)
        {
            ButtonProductNew.Visibility = Visibility.Collapsed;
            ButtonProductUpdate.Visibility = Visibility.Collapsed;
            ButtonProductSync.Visibility = Visibility.Collapsed;
            TextBlockProductUpdateStatus.Visibility = Visibility.Collapsed;
            TextBlockProductSyncStatus.Visibility = Visibility.Collapsed;

            StackPanelProductEdit.Visibility = Visibility.Visible;
            ButtonProductDelete.Visibility = Visibility.Collapsed;
            ButtonProductReturn.Visibility = Visibility.Collapsed;
            ButtonProductSave.IsEnabled = false;

            SetAllProductTextBoxesToEmpty();
            EnableTextBoxesByProductAndMode(new Product(), ProductMode.New);
        }

        /// <summary>
        /// Save the product to the storage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonProductSave_OnClick(object sender, RoutedEventArgs e)
        {
            InventoryInfo invetoryInfoItem = null;
           // Models.Product savedProduct = null;
            var inventoryDateTime = DateTime.Now;
            Product productToUpdate = null;

            if (_selectedStorageProduct != null)
            {
                productToUpdate = _selectedStorageProduct;
                _selectedStorageProduct.Name = TextBoxProductName.Text;
                _selectedStorageProduct.Price = Convert.ToDecimal(TextBoxProductPrice.Text);
                _selectedStorageProduct.Stock = Convert.ToInt32(TextBoxProductStock.Text);
                

                invetoryInfoItem = InventoryHelper.BuildInventoryItemFromProduct(_selectedStorageProduct, inventoryDateTime);

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
                        movie.PlayTime = string.IsNullOrWhiteSpace(TextBoxProductPlayTime.Text) ? 0 : int.Parse(TextBoxProductPlayTime.Text);
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
                productToUpdate = newProduct;
                newProduct.Id = Convert.ToInt32(TextBoxProductId.Text);
                newProduct.Name = TextBoxProductName.Text;
                newProduct.Price = Convert.ToDecimal(TextBoxProductPrice.Text);
                newProduct.Stock = Convert.ToInt32(TextBoxProductStock.Text);

                invetoryInfoItem = InventoryHelper.BuildInventoryItemFromProduct(newProduct, inventoryDateTime);

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
                        movie.PlayTime = string.IsNullOrWhiteSpace(TextBoxProductPlayTime.Text) ? 0 : int.Parse(TextBoxProductPlayTime.Text);
                        break;
                    case Game game:
                        game.Platform = TextBoxProductPlatform.Text;
                        break;
                }

                Products.Add(newProduct);
                FilteredProducts.Add(newProduct);
            }

            ButtonProductNew.Visibility = Visibility.Visible;
            ButtonProductUpdate.Visibility = Visibility.Visible;
            ButtonProductSync.Visibility = Visibility.Visible;
            TextBlockProductUpdateStatus.Visibility = Visibility.Visible;
            TextBlockProductSyncStatus.Visibility = Visibility.Visible;
            StackPanelProductEdit.Visibility = Visibility.Collapsed;

            _selectedStorageProduct = null;
            ListViewStorage.SelectedIndex = -1;

            // update the inventory list
            if (invetoryInfoItem != null)
            {
                InventoryList.Add(invetoryInfoItem);
            }

            // update the stock at remote storage
            SyncLocalProductsToRemoteStorage(new List<Product> { productToUpdate });
        }

        /// <summary>
        /// Event handler for the selection of a product in the storage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewStorage_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedStorageProduct = ((Product)ListViewStorage.SelectedItem);

            // do nothing if no product is selected
            if (_selectedStorageProduct == null)
                return;

            ButtonProductNew.Visibility = Visibility.Collapsed;
            ButtonProductUpdate.Visibility = Visibility.Collapsed;
            ButtonProductSync.Visibility = Visibility.Collapsed;
            TextBlockProductUpdateStatus.Visibility = Visibility.Collapsed;
            TextBlockProductSyncStatus.Visibility = Visibility.Collapsed;
            StackPanelProductEdit.Visibility = Visibility.Visible;
            ButtonProductDelete.Visibility = Visibility.Visible;
            ButtonProductReturn.Visibility = Visibility.Visible;

            EnableTextBoxesByProductAndMode(_selectedStorageProduct, ProductMode.Edit);
            PopulateTextBoxesInputFormByProduct(_selectedStorageProduct);
        }

        /// <summary>
        /// Add text information into the input form based on selected product
        /// </summary>
        /// <param name="product"></param>
        private void PopulateTextBoxesInputFormByProduct(Product product)
        {
            if (product == null)
                return;

            SetAllProductTextBoxesToEmpty();

            TextBoxProductId.Text = product.Id.ToString();
            TextBoxProductName.Text = product.Name;
            TextBoxProductPrice.Text = product.Price.ToString();
            TextBoxProductStock.Text = product.Stock.ToString();


            ComboBoxProductType.Visibility = Visibility.Collapsed;
            ComboBoxProductType.IsEditable = false;

            switch (product)
            {
                case Book book:
                    TextBoxProductAuthor.Text = book.Author;
                    TextBoxProductGenre.Text = book.Genre;
                    TextBoxProductFormat.Text = book.Format;
                    TextBoxProductLanguage.Text = book.Language;
                    break;
                case Movie movie:
                    TextBoxProductFormat.Text = movie.Format;
                    TextBoxProductPlayTime.Text = movie.PlayTime.ToString();
                    break;
                case Game game:
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

        /// <summary>
        /// Get the product type based on the selected value
        /// </summary>
        /// <param name="selectedValue"></param>
        /// <returns></returns>

        private static Product GetProductTypeBySelectionName(string selectedValue)
        {
            var seletedProductType = new Product();

            if (selectedValue == Constants.ProuctTypesTranslaton.Book)
            {
                seletedProductType = new Book();
            }
            else if (selectedValue == Constants.ProuctTypesTranslaton.Movie)
            {
                seletedProductType = new Movie();
            }
            else if (selectedValue == Constants.ProuctTypesTranslaton.Game)
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
                ComboBoxProductType.Visibility = Visibility.Visible;
                TextBoxProductType.Text = "";
            }
            else
            {
                TextBoxProductType.Text = product.GetTypeNameTranslation();
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonProductCancel_OnClick(object sender, RoutedEventArgs e)
        {
            _selectedStorageProduct = null;

            ButtonProductNew.Visibility = Visibility.Visible;
            ButtonProductUpdate.Visibility = Visibility.Visible;
            ButtonProductSync.Visibility = Visibility.Visible;
            TextBlockProductUpdateStatus.Visibility = Visibility.Visible;
            TextBlockProductSyncStatus.Visibility = Visibility.Visible;
            StackPanelProductEdit.Visibility = Visibility.Collapsed;

        }

        /// <summary>
        /// Delete selected product from storage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    // https://docs.microsoft.com/en-us/windows/uwp/design/controls-and-patterns/dialogs-and-flyouts/dialogs
                    var dialog = new ContentDialog
                    {
                        Title = $"Ta bort produkt: {_selectedStorageProduct.Name}",
                        Content = "Det finns flera produkter på lager, är du säker på att du vill ta bort denna?",
                        PrimaryButtonText = "Ja",
                        SecondaryButtonText = "Nej",
                        CloseButtonText = "Avbryt"
                    };

                    dialog.PrimaryButtonClick += async (s, args) =>
                    {
                        RemoveSelectedProductFromAllLists(_selectedStorageProduct);
                        _selectedStorageProduct = null;
                        
                        ButtonProductNew.Visibility = Visibility.Visible;
                        ButtonProductUpdate.Visibility = Visibility.Visible;
                        ButtonProductSync.Visibility = Visibility.Visible;
                        TextBlockProductUpdateStatus.Visibility = Visibility.Visible;
                        TextBlockProductSyncStatus.Visibility = Visibility.Visible;
                        StackPanelProductEdit.Visibility = Visibility.Collapsed;
                        ListViewStorage.SelectedIndex = -1;

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

        /// <summary>
        /// Remove selected product from all lists
        /// </summary>
        /// <param name="product"></param>
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

        /// <summary>
        /// Check the input for the product
        /// </summary>
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

        /// <summary>
        /// Check the input for product id
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxProductId_OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            var textInput = textBox?.Text;

            textBox.DisplayValidationColor(textBox.IsValidProductId(textInput, Products.ToList(), _selectedStorageProduct));

            CheckValidProductInput();
        }

        /// <summary>
        /// Check the input for product name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxProductName_OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            var textInput = textBox?.Text;

            textBox.DisplayValidationColor(textBox.IsValidProductName(textInput));

            CheckValidProductInput();
        }


        /// <summary>
        /// CHekc the input for product price
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxProductPrice_OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            var textInput = textBox?.Text;

            textBox.DisplayValidationColor(textBox.IsValidProductPrice(textInput));

            CheckValidProductInput();
        }

        /// <summary>
        /// Check the input for product stock
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxProductStock_OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            var textInput = textBox?.Text;

            textBox.DisplayValidationColor(textBox.IsValidProductStock(textInput));

            CheckValidProductInput();
        }

        /// <summary>
        /// Check the input for product playtime
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxProductPlayTime_OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            var textInput = textBox?.Text;

            textBox.DisplayValidationColor(textBox.IsValidProductPlaytime(textInput));

            CheckValidProductInput();

        }


        /// <summary>
        /// Return a product
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonProductReturn_OnClick(object sender, RoutedEventArgs e)
        {
            if (_selectedStorageProduct != null)
            {
                // https://docs.microsoft.com/en-us/windows/uwp/design/controls-and-patterns/dialogs-and-flyouts/dialogs
                var dialog = new ContentDialog
                {
                    Title = $"Retur av produkt: {_selectedStorageProduct.Name}",
                    Content = "Vill du lämna tillbaka en produkt",
                    PrimaryButtonText = "Ja",
                    SecondaryButtonText = "Nej",
                    CloseButtonText = "Avbryt"
                };

                dialog.PrimaryButtonClick += async (s, args) =>
                {

                    if (_selectedStorageProduct.Stock + 1 <= Int32.MaxValue)
                    {
                        _selectedStorageProduct.Stock += 1;
                        TextBoxProductStock.Text = _selectedStorageProduct.Stock.ToString();
                    }

                };

                dialog.SecondaryButtonClick += (s, args) =>
                {
                    // do nothing
                };

                await dialog.ShowAsync();
            }
        }

        /// <summary>
        /// Update the local products from remote storage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonProductUpdate_OnClick(object sender, RoutedEventArgs e)
        {
            UpdateLocalProductsFromRemoteStorage();
        }

        /// <summary>
        /// Sync the local products to remote storage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonProductSync_OnClick(object sender, RoutedEventArgs e)
        {
            SyncLocalProductsToRemoteStorage(Products.ToList());
        }


        /// <summary>
        /// Syncth local products stock to remote storage
        /// </summary>
        private async void SyncLocalProductsToRemoteStorage(List<Product> productsToSync)
        {
            var productSyncCount = 0;

            try
            {
                foreach (var product in productsToSync)
                {
                    // this can be rewritten to more dynamic solution
                    // to enable this you can get the full list of products from the remote storage and only make use of the ids that exists in the remote storage
                    // for now we know that there is no id higher than 15 or below 1
                    if (product.Id > 15)
                    {
                        continue;
                    }
                    await new StorageService().UpdateProductStockAsync(product.Id, product.Stock);
                    productSyncCount++;
                }

                TextBlockProductSyncStatus.Text = $"Synk mot lager: {DateTime.Now.ToString(DateFormats.DateTimeFormat)}\nAntal produkter: {productSyncCount}";
                TextBlockProductSyncStatus.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Green);

                PivotItemLager.Header = "Lager"; // ok text
            }
            catch
            {
                TextBlockProductSyncStatus.Text = $"Fel på produktsynk mot lager {DateTime.Now.ToString(DateFormats.DateTimeFormat)}";

                // set color on TextBlockProductUpdateStatus to red
                TextBlockProductSyncStatus.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Red);
                PivotItemLager.Header = "Lager(!)"; // indicate that there is an error
            }
        }


        /// <summary>
        /// Update the local products from remote storage and update inventory list
        /// </summary>
        private async void UpdateLocalProductsFromRemoteStorage()
        {
            try
            {
                var productUpdateCount = 0;
                var productsThatShouldBeInventoried = new List<Product>();

                var remoteStorageProducts = await Task.Run(() => new StorageService().GetProductsAsync());

                foreach (var remoteProduct in remoteStorageProducts)
                {
                    // if the products in local Products list then update the stock and price, otherwise add the product to the list
                    if (Products.Any(p => p.Id == remoteProduct.Id))
                    {
                        var localProduct = Products.First(p => p.Id == remoteProduct.Id);
                        localProduct.Stock = remoteProduct.Stock;
                        localProduct.Price = remoteProduct.Price;

                        if (localProduct.Reserved > localProduct.Stock)
                        {
                            localProduct.Reserved = localProduct.Stock;
                        }

                        productsThatShouldBeInventoried.Add(localProduct);
                    }
                    else
                    {
                        // create local product from remote product and add to the products list
                        var newProduct = ProductHelper.CreateProductFromRemoteProduct(remoteProduct);
                        productsThatShouldBeInventoried.Add(newProduct);
                        Products.Add(newProduct);
                        FilteredProducts.Add(newProduct);
                    }

                    productUpdateCount++;
                }




                // build inventory list from products that should be inventoried and update the inventory list
                var newInventoryList = InventoryHelper.BuildInventoryItemsFromProducts(productsThatShouldBeInventoried, DateTime.Now);
                foreach (var item in newInventoryList)
                {
                    InventoryList.Add(item);
                }

                TextBlockProductUpdateStatus.Text = $"Uppdatering från lagret {DateTime.Now.ToString(DateFormats.DateTimeFormat)}\nAntal produkter: {productUpdateCount}";
                TextBlockProductUpdateStatus.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Green);
                PivotItemLager.Header = "Lager"; // ok text
            }
            catch
            {

                TextBlockProductUpdateStatus.Text = $"Fel på produktuppdatering från lagret {DateTime.Now.ToString(DateFormats.DateTimeFormat)}";
                TextBlockProductUpdateStatus.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Red);
                PivotItemLager.Header = "Lager(!)"; // indicate that there is an error
            }
        }

        #endregion

        #region Pivot Report 

        /// <summary>
        /// Event handler for the button to get the report data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonGetReportData_Click(object sender, RoutedEventArgs e)
        {
            var orderItems = new OrderRepository().GetOrders();

            if (ComboBoxReportType.SelectedIndex == 0)
            {
                TextBlockReportHeader.Text = "Top-10-lista per år och månad";
                TextBoxReportResult.Text = ReportItemHelper.GetTop10MostSoldProductsPerYearAndMonthReport(orderItems);
            }
            else if (ComboBoxReportType.SelectedIndex == 1)
            {
                TextBlockReportHeader.Text = "Total försäljning av produkter per år och månad";
                TextBoxReportResult.Text = ReportItemHelper.GetTotalSalesPerYearAndMonthReport(orderItems);
            }
            else
            {
                TextBlockReportHeader.Text = "Ingen rapportdata är vald";
                TextBoxReportResult.Text = "";
            }
        }

        #endregion

        #region Pivot Historisk lagerstatus
        /// <summary>
        /// Display the historic status for a product in a chartview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewHistoricStatus_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedProduct = (Product)ListViewHistoricStatus.SelectedItem;
            _selectedInventoryProduct = selectedProduct;
            DisplayInventoryChartView();
        }

        /// <summary>
        /// Event handler for the selection of the historic type, refresh the chartview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBoxHistoricType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisplayInventoryChartView();
        }


        /// <summary>
        /// Get the current inventory value labels that should be displayed in the chart
        /// </summary>
        /// <returns></returns>
        private string GetCurrentInventoryValueLabels()
        {
            if (ComboBoxHistoricType.SelectedIndex == 0)
                return InventoryValueLabels.Stock;

            if (ComboBoxHistoricType.SelectedIndex == 1)
                return InventoryValueLabels.Price;

            return InventoryValueLabels.Stock;
        }



        /// <summary>
        ///  TODO: Display the historic status for a product in a chartview
        /// </summary>
        private void DisplayInventoryChartView()
        {
            if (_selectedInventoryProduct == null)
            {
                return;
            }

            var displayMode = GetCurrentInventoryValueLabels();

            var chartEntries = new List<ChartEntry>();

            
            // filter the inventory list for the selected product
            var filterList = InventoryList.Where(i => i.Id == _selectedInventoryProduct.Id).ToList();

            // build a ChartEntry list to display in chartview. Only show the last 15 entries
            foreach (var itemInventoryInfo in filterList.OrderBy(x => x.DateTime).TakeLast(15))
            {
                chartEntries.Add(new ChartEntry(itemInventoryInfo.Stock)
                {
                    Label = $"{itemInventoryInfo.DateTime.ToString(DateFormats.DateTimeFormat)}",
                    ValueLabel = displayMode == InventoryValueLabels.Stock ?  itemInventoryInfo.Stock.ToString() : itemInventoryInfo.Price.ToString(),
                    Color = displayMode == InventoryValueLabels.Stock ? SKColor.Parse("#66CC00") : SKColor.Parse("#3399FF"),
                });
            }
            
            // https://github.com/microcharts-dotnet/Microcharts/wiki
            // https://github.com/microcharts-dotnet/Microcharts/wiki/BarChart
            var barChart = new BarChart { Entries = chartEntries };
            barChart.IsAnimated = true;
            
            chartView.Chart = barChart;
            chartView.Width = 50 * chartEntries.Count;
            
            if (displayMode == InventoryValueLabels.Stock)
            {
                TextBlockChartHeader.Text = $"Lagerhistorik för {_selectedInventoryProduct?.Name}";
            }
            else
            {
                TextBlockChartHeader.Text = $"Prishistorik för {_selectedInventoryProduct?.Name}";
            }

        }

    
        #endregion

        #region Printer Handling
        /// <summary>
        /// Print the receipt for the last purchase
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonBasketPrint_OnClick(object sender, RoutedEventArgs e)
        {
            printReceiptPage = new PrintReceiptPage(); // custom print page
            printReceiptPage.SetRecieptInfo(lastestReceiptInfo); // set the receipt info

            pages.Clear();
            pages.Add(printReceiptPage);
            await PrintManager.ShowPrintUIAsync(); // show the print dialog
        }

        /// <summary>
        /// Build the receipt print for the last purchase
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        private string BuildRecieptPrint(List<Product> products)
        {
            var reciept = new StringBuilder();

            reciept.AppendLine("-------------------------------");
            reciept.AppendLine("Vara | Pris | Antal");
            reciept.AppendLine("-------------------------------");

            foreach (var product in products)
            {
                reciept.AppendLine($"{product.Name} | {product.Price} kr | {product.Reserved} stycken");
            }
            reciept.AppendLine("-------------------------------");
            reciept.AppendLine($"Total pris: {products.Sum(p => p.Price * p.Reserved)} kr");
            reciept.AppendLine($"Total antal varor: {products.Sum(p => p.Reserved)} stycken");
            reciept.AppendLine("-------------------------------");
            reciept.AppendLine();
            reciept.AppendLine("Tack för att du handlade hos oss!");
            return reciept.ToString();
        }

        // Code borrowed and adjuset from follwing sources:
        // https://stackoverflow.com/questions/36207708/print-multiple-pages-from-a-uwa
        // https://learn.microsoft.com/en-us/windows/uwp/devices-sensors/print-from-your-app


        // used for printing receipt
        PrintDocument printDocument;
        IPrintDocumentSource printDocumentSource;
        List<Page> pages = new List<Page>();
        PrintReceiptPage printReceiptPage = new PrintReceiptPage();
        string lastestReceiptInfo = "";


        /// <summary>
        ///  Register the print manager for the current view and add the print task requested event handler
        /// </summary>
        public void RegisterForPrinting()
        {
            printDocument = new PrintDocument();
            printDocumentSource = printDocument.DocumentSource;
            pages.Add(printReceiptPage);
            printDocument.GetPreviewPage += GetPrintPreviewPage;
            printDocument.AddPages += AddPrintPages;
            PrintManager printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested += PrintTaskRequested;
        }

        /// <summary>
        /// Add the print pages to the print document
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddPrintPages(object sender, AddPagesEventArgs e)
        {
            foreach (var page in pages)
            {
                printDocument.AddPage(page);
            }
            printDocument.AddPagesComplete();
        }

        /// <summary>
        /// Set the print preview page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetPrintPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            printDocument.SetPreviewPage(1, printReceiptPage);
            printDocument.SetPreviewPageCount(pages.Count, PreviewPageCountType.Final);
        }

        /// <summary>
        /// Set the print task requested event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs e)
        {
            PrintTask printTask = null;
            printTask = e.Request.CreatePrintTask("Receipt Print Job", sourceRequested =>
            {
                sourceRequested.SetSource(printDocumentSource);
            });
        }
        #endregion

        #region Application events

        /// <summary>
        /// event handler for app exit
        /// </summary>
        public void OnAppExit()
        {
            // store the products in a csv file
            new Repositories.ProductsRepository().WriteProductsToDataFile(Products.ToList());
        }
        /// <summary>
        /// Executed when the page is navigated to. COnfigure the print manager
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            RegisterForPrinting();
        }

        /// <summary>
        /// Executed when the page is navigated from. Unregister the print manager
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            PrintManager printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested -= PrintTaskRequested;
        }

        #endregion

        
    }
}