using System;
using System.Collections.Generic;
using BusinessSystem.Models;

namespace BusinessSystem.Helpers
{
    public static class InventoryHelper
    {
        public static List<InventoryInfo> BuildInventoryItemsFromProducts(List<Product> products, DateTime inventoryDateTime)
        {


            var inventoryItems = new List<InventoryInfo>();

            if (products == null)
            {
                return inventoryItems;
            }

            foreach (var product in products)
            {
                var inventoryItem = new InventoryInfo
                {
                    DateTime = inventoryDateTime,
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Stock = product.Stock
                };

                inventoryItems.Add(inventoryItem);
            }

            return inventoryItems;
        }
    }
}
