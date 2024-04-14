using BusinessSystem.Models;
using BusinessSystem.Models.Constants;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Windows.Storage;

namespace BusinessSystem.Repositories
{
    public class ProductsRepository
    {
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        private const string ProductsInitialDataCsv = @"\Repositories\data\products_initial_data.csv";
        const string ProductsDataCsv = "products_data.csv";

        public ProductsRepository()
        {
            // if no product data exist in 'local folder', then copy initial data to 'local folder'
            if (!CheckIfFileExists(ProductsDataCsv))
            {
                CopyInitialProductDataToLocalFolder();
            }
        }

        #region Helper methods

        private bool CopyInitialProductDataToLocalFolder()
        {
            try
            {
                // Get the file from the installed location
                var initialDataCsvFileSource = Windows.ApplicationModel.Package.Current.InstalledLocation.Path + ProductsInitialDataCsv;

                var sourceFile = StorageFile.GetFileFromPathAsync(initialDataCsvFileSource).AsTask().GetAwaiter().GetResult();


                // Copy the file to the local folder
                // docs: https://docs.microsoft.com/en-us/uwp/api/windows.storage.storagefile.copyasync
                sourceFile.CopyAsync(localFolder, ProductsDataCsv, NameCollisionOption.ReplaceExisting).AsTask().GetAwaiter().GetResult();



                Debug.WriteLine($"File copied successfully to LocalFolder with name: {ProductsDataCsv}");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error copying file: {ex.Message}");
                return false;
            }
        }

        private bool CheckIfFileExists(string fileName)
        {
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile file = localFolder.GetFileAsync(fileName).AsTask().GetAwaiter().GetResult();
                return true; // File exists
            }
            catch (FileNotFoundException)
            {
                return false; // File does not exist
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Debug.WriteLine($"Error checking file existence: {ex.Message}");
                return false;
            }
        }
        #endregion


        /// <summary>
        /// Write products to local data file (csv file)
        /// </summary>
        /// <param name="products"></param>
        public void WriteProductsToDataFile(List<Product> products)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = localFolder.CreateFileAsync(ProductsDataCsv, CreationCollisionOption.ReplaceExisting).AsTask().GetAwaiter().GetResult();

            using (StreamWriter writer = new StreamWriter(file.OpenStreamForWriteAsync().GetAwaiter().GetResult(), System.Text.Encoding.UTF8))
            {
                writer.WriteLine("Type,Id,Name,Price,Author,Platform,Genre,Format,Language,PlayTime,Stock");

                foreach (var product in products)
                {
                    switch (product)
                    {
                        case Book book:
                            writer.WriteLine($"{Constants.ProductTypes.Book},{book.Id},{book.Name},{book.Price},{book.Author},,,{book.Format},{book.Language},,{book.Stock}");
                            break;
                        case Movie movie:
                            writer.WriteLine($"{Constants.ProductTypes.Movie},{movie.Id},{movie.Name},{movie.Price},,,,{movie.Format},,{movie.PlayTime},{movie.Stock}");
                            break;
                        case Game game:
                            writer.WriteLine($"{Constants.ProductTypes.Game},{game.Id},{game.Name},{game.Price},,,{game.Platform},,,,{game.Stock}");
                            break;
                        case Product basicProduct:
                            writer.WriteLine($"{Constants.ProductTypes.Product},{basicProduct.Id},{basicProduct.Name},{basicProduct.Price},,,,,,,{basicProduct.Stock}");
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Get products from local data file (csv file)
        /// </summary>
        /// <returns></returns>
        public List<Product> GetProducts()
        {
            var products = new List<Product>();
            var localFolder = ApplicationData.Current.LocalFolder;
            var file = localFolder.GetFileAsync(ProductsDataCsv).AsTask().Result;

            var lines = File.ReadAllLines(file.Path, Encoding.UTF8);

            var firstElementSkipped = false;

            foreach (var line in lines)
            {
                if (!firstElementSkipped)
                {
                    firstElementSkipped = true;
                    continue;
                }

                var columns = line.Split(',');

                switch (columns[0])
                {
                    case Constants.ProductTypes.Book:
                        {
                            var product = new Book
                            {
                                Id = int.Parse(columns[1]),
                                Name = columns[2],
                                Price = decimal.Parse(columns[3]),
                                Author = columns[4],
                                Genre = columns[6],
                                Format = columns[7],
                                Language = columns[8],
                                Stock = string.IsNullOrWhiteSpace(columns[10]) ? 0 : int.Parse(columns[10])
                            };

                            products.Add(product);
                            break;
                        }
                    case Constants.ProductTypes.Movie:
                        {
                            var product = new Movie
                            {
                                Id = int.Parse(columns[1]),
                                Name = columns[2],
                                Price = decimal.Parse(columns[3]),
                                Format = columns[7],
                                PlayTime = string.IsNullOrWhiteSpace(columns[9]) ? 0 : int.Parse(columns[9]),
                                Stock = string.IsNullOrWhiteSpace(columns[10]) ? 0 : int.Parse(columns[10])
                            };

                            products.Add(product);
                            break;
                        }
                    case Constants.ProductTypes.Game:
                        {
                            var product = new Game
                            {
                                Id = int.Parse(columns[1]),
                                Name = columns[2],
                                Price = decimal.Parse(columns[3]),
                                Platform = columns[5],
                                Stock = string.IsNullOrWhiteSpace(columns[10]) ? 0 : int.Parse(columns[10])
                            };

                            products.Add(product);
                            break;
                        }
                    case Constants.ProductTypes.Product:
                        {
                            var product = new Product
                            {
                                Id = int.Parse(columns[1]),
                                Name = columns[2],
                                Price = decimal.Parse(columns[3]),
                                Stock = string.IsNullOrWhiteSpace(columns[10]) ? 0 : int.Parse(columns[10])
                            };

                            products.Add(product);
                            break;
                        }
                }
            }
            return products;
        }
    }
}