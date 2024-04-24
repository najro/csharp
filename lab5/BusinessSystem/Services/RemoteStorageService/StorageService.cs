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
    }
}
