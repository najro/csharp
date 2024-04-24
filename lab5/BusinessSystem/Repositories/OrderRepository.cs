using BusinessSystem.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Windows.Storage;

namespace BusinessSystem.Repositories
{
    /// <summary>
    /// Read and write orders to local data file (csv file)
    /// https://learn.microsoft.com/en-us/uwp/api/windows.storage.storagefolder?view=winrt-22621
    /// https://learn.microsoft.com/en-us/dotnet/api/system.io.file.readalllines?view=net-8.0
    /// </summary>
    public class OrderRepository
    {
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        const string OrderDataCsv = "order_data_v2.csv";

        public OrderRepository()
        {

        }


        private List<OrderItem> BuildOrderItemsFromCsv(string[] lines)
        {
            var orderItems = new List<OrderItem>();

            var firstElementSkipped = false;

            foreach (var line in lines)
            {
                if (!firstElementSkipped)
                {
                    firstElementSkipped = true;
                    continue;
                }

                var columns = line.Split(',');

                var orderItem = new OrderItem
                {
                    // OrderId = int.Parse(columns[0]),
                    OrderDate = DateTime.Parse(columns[1]),
                    ProductId = int.Parse(columns[2]),
                    Name = columns[3],
                    Type = columns[4],
                    Quantity = int.Parse(columns[5]),
                    Price = decimal.Parse(columns[6])
                };

                orderItems.Add(orderItem);
            }

            return orderItems;
        }

        public List<OrderItem> GetOrders()
        {
            var orderItems = new List<OrderItem>();

            if (!CheckIfFileExists(OrderDataCsv))
            {
                return orderItems;
            }

            var localFolder = ApplicationData.Current.LocalFolder;
            var file = localFolder.GetFileAsync(OrderDataCsv).AsTask().Result;

            var lines = File.ReadAllLines(file.Path, Encoding.UTF8);

            return BuildOrderItemsFromCsv(lines);
        }


        public void WriteOrderItemsToDataFile(List<OrderItem> products)
        {


            var localFolder = ApplicationData.Current.LocalFolder;
            var file = localFolder.CreateFileAsync(OrderDataCsv, CreationCollisionOption.OpenIfExists).AsTask().GetAwaiter().GetResult();


            using (StreamWriter writer = new StreamWriter(file.OpenStreamForWriteAsync().GetAwaiter().GetResult(), System.Text.Encoding.UTF8))
            {

                // Check if file contains data
                if (writer.BaseStream.Length == 0)
                {
                    // Write the header only if the file is empty
                    writer.WriteLine("OrderId,OrderDate,ProductId,Name,Type,Quantity,Price");
                }

                // Move the file pointer to the end of the file to append data
                writer.BaseStream.Seek(0, SeekOrigin.End);

                foreach (var item in products)
                {
                    writer.WriteLine($"{item.OrderId.ToString()},{item.OrderDate.ToString("yyyy-MM-dd HH:mm:ss")},{item.ProductId},{item.Name},{item.Type},{item.Quantity},{item.Price}");
                }
            }
        }

        private bool CheckIfFileExists(string fileName)
        {
            try
            {
                var localFolder = ApplicationData.Current.LocalFolder;
                var file = localFolder.GetFileAsync(fileName).AsTask().GetAwaiter().GetResult();
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
    }
}
