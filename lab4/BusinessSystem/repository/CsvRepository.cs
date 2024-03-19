using BusinessSystem.Models;
using System.Collections.ObjectModel;

namespace BusinessSystem.repository
{
    public class CsvRepository
    {
        //private ObservableCollection<Product> products;

        public CsvRepository()
        {
            //products = ReadProductsFromFile();
        }


        public ObservableCollection<Product> ReadProductsFromFile()
        {
            var products = new ObservableCollection<Product>();

            var lines = System.IO.File.ReadAllLines("repository/products.csv");


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
                            Language = columns[8]
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
                            PlayTime = columns[9]
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
                            Platform = columns[5]
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

