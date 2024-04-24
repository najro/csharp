using BusinessSystem.Services.RemoteStorageService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BusinessSystem.Services.RemoteStorageService
{
    public class StorageService
    {
        private const string storageApiUrl = "https://hex.cse.kau.se/~jonavest/csharp-api/";
        
        private const string storageUpdateAction = "?action=update&id={0}&stock={1}";

        public async Task<List<Product>> GetProductsAsync()
        {
            List<Product> products = new List<Product>();
            try
            {
                // https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-8.0
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(storageApiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string xmlString = await response.Content.ReadAsStringAsync();

                        //https://learn.microsoft.com/en-us/dotnet/api/system.xml.linq.xelement?view=net-8.0
                        XDocument doc = XDocument.Parse(xmlString);

                        foreach (XElement element in doc.Descendants("book")
                                     .Union(doc.Descendants("game"))
                                     .Union(doc.Descendants("movie")))
                        {
                            Product product = new Product
                            {
                                Id = int.Parse(element.Element("id").Value),
                                Name = element.Element("name").Value,
                                Price = decimal.Parse(element.Element("price").Value),
                                Stock = int.Parse(element.Element("stock").Value),
                                Genre = element.Element("genre")?.Value,
                                Format = element.Element("format")?.Value,
                                Language = element.Element("language")?.Value,
                                Platform = element.Element("platform")?.Value,
                                Playtime = element.Element("playtime") != null ? int.Parse(element.Element("playtime").Value) : (int?)null
                            };

                            products.Add(product);
                        }
                    }
                    else
                    {
                       throw new Exception($"An error occurred: {response.StatusCode}");
                    }
                }

                return products;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred: {ex.Message}");
            }
        }


        public async Task UpdateProductStockAsync(int id, int stock)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = string.Format(storageUpdateAction, id, stock);
                    HttpResponseMessage response = await client.GetAsync(storageApiUrl + url);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"An error occurred: {response.StatusCode}");
                    }

                    Console.WriteLine($"Stock for product with id {id} updated to {stock}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred: {ex.Message}");
            }
        }
    }
}
