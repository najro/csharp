using BusinessSystem.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Windows.Storage;

namespace BusinessSystem.repository
{
    public class CsvRepository
    {
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;

        private const string ProductsInitialDataCsv = @"\repository\products_initial_data.csv";
        const string ProductsDataCsv = "products_data.csv";
        const string OrderDataCsv = "order_data.csv";

        public CsvRepository()
        {

            // if not product data exist in local folder, then copy initial data to local folder
            if (!CheckIfFileExists(ProductsDataCsv))
            {
                CopyInitialProductDataToLocalFolder();

            }
        }

        public static bool CopyInitialProductDataToLocalFolder()
        {
            try
            {
                var initialDataCsvFileSource = Windows.ApplicationModel.Package.Current.InstalledLocation.Path + ProductsInitialDataCsv;

                var sourceFile = StorageFile.GetFileFromPathAsync(initialDataCsvFileSource).AsTask().GetAwaiter().GetResult();

                var localFolder = ApplicationData.Current.LocalFolder;

                sourceFile.CopyAsync(localFolder, ProductsDataCsv, NameCollisionOption.ReplaceExisting).AsTask().GetAwaiter().GetResult();

                Console.WriteLine($"File copied successfully to LocalFolder with name: {ProductsDataCsv}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error copying file: {ex.Message}");
                return false;
            }
        }

        public static bool CheckIfFileExists(string fileName)
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
                Console.WriteLine($"Error checking file existence: {ex.Message}");
                return false;
            }
        }


        // store observable collection of products to csv file
        public void WriteProductsToDataFile(ObservableCollection<Product> products)
        {

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = localFolder.CreateFileAsync(ProductsDataCsv, CreationCollisionOption.ReplaceExisting).AsTask().GetAwaiter().GetResult();


            using (StreamWriter writer = new StreamWriter(file.OpenStreamForWriteAsync().GetAwaiter().GetResult()))
            {
                writer.WriteLine("Type,Id,Name,Price,Author,Platform,Genre,Format,Language,PlayTime,Stock");

                foreach (var product in products)
                {
                    switch (product)
                    {
                        case Book book:
                            writer.WriteLine($"Book,{book.Id},{book.Name},{book.Price},{book.Author},,,{book.Format},{book.Language},,{book.Stock}");
                            break;
                        case Movie movie:
                            writer.WriteLine($"Movie,{movie.Id},{movie.Name},{movie.Price},,,,{movie.Format},,{movie.PlayTime},{movie.Stock}");
                            break;
                        case Game game:
                            writer.WriteLine($"Game,{game.Id},{game.Name},{game.Price},,,{game.Platform},,,,{game.Stock}");
                            break;
                        case Product basicProduct:
                            writer.WriteLine($"Product,{basicProduct.Id},{basicProduct.Name},{basicProduct.Price},,,,,,,{basicProduct.Stock}");
                            break;
                    }
                }
            }
        }

        public ObservableCollection<Product> ReadProductsFromDataFile()
        {
            var products = new ObservableCollection<Product>();
            var localFolder = ApplicationData.Current.LocalFolder;
            var file = localFolder.GetFileAsync(ProductsDataCsv).AsTask().Result;

            var lines = File.ReadAllLines(file.Path);

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
                    case "Book":
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
                    case "Movie":
                        {
                            var product = new Movie
                            {
                                Id = int.Parse(columns[1]),
                                Name = columns[2],
                                Price = decimal.Parse(columns[3]),
                                Format = columns[7],
                                PlayTime = string.IsNullOrWhiteSpace(columns[9]) ? 0 : int.Parse(columns[9])
                            };

                            products.Add(product);
                            break;
                        }
                    case "Game":
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
                    case "Product":
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



      
        public void WriteOrderItemsToDataFile(List<OrderItem> products)
        {

            var localFolder = ApplicationData.Current.LocalFolder;
            var file = localFolder.CreateFileAsync(OrderDataCsv, CreationCollisionOption.OpenIfExists).AsTask().GetAwaiter().GetResult();


            using (StreamWriter writer = new StreamWriter(file.OpenStreamForWriteAsync().GetAwaiter().GetResult()))
            {

                // Check if file contains data
                if (writer.BaseStream.Length == 0)
                {
                    // Write the header only if the file is empty
                    writer.WriteLine("OrderId,OrderDate,ProductId,Name,Type,Quantity");
                }

                // Move the file pointer to the end of the file to append data
                writer.BaseStream.Seek(0, SeekOrigin.End);

                foreach (var item in products)
                {
                    writer.WriteLine($"{item.OrderId.ToString()}, {item.OrderDate.ToString("yyyy-MM-dd HH:mm:ss")}, {item.ProductId},{item.Name},{item.Type},{item.Quantity}");
                }
            }
        }
    }
}

