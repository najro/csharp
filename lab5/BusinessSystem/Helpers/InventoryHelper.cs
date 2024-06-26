﻿using System;
using System.Collections.Generic;
using BusinessSystem.Models;

namespace BusinessSystem.Helpers
{
    // Helper class for inventory items
    public static class InventoryHelper
    {

        public static InventoryInfo BuildInventoryItemFromProduct(Product product, DateTime inventoryDateTime)
        {
            if (product == null)
            {
                return null;
            }

            return CreateInventoryItem(product, inventoryDateTime);

        }
        public static List<InventoryInfo> BuildInventoryItemsFromProducts(List<Product> products, DateTime inventoryDateTime)
        {
            var inventoryItems = new List<InventoryInfo>();

            if (products == null)
            {
                return inventoryItems;
            }

            foreach (var product in products)
            {
                var inventoryItem = CreateInventoryItem(product, inventoryDateTime);
                inventoryItems.Add(inventoryItem);
            }

            return inventoryItems;
        }

        private static InventoryInfo CreateInventoryItem(Product product, DateTime inventoryDateTime)
        {
            var inventoryItem = new InventoryInfo
            {
                DateTime = inventoryDateTime,
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock
            };
            return inventoryItem;
        }
    }
}
