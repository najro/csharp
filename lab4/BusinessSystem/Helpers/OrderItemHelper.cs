using BusinessSystem.Models;
using System;
using System.Collections.Generic;

namespace BusinessSystem.Helpers
{
    public static class OrderItemHelper
    {
        // Build order items from products with order id and order date        
        public static List<OrderItem> BuildOrderItemsFromProducts(List<Product> products, Guid orderId, DateTime orderDate)
        {
            var orderItems = new List<OrderItem>();

            if (products == null)
            {
                return orderItems;
            }

            foreach (var product in products)
            {
                var orderItem = new OrderItem
                {
                    OrderDate = orderDate,
                    OrderId = orderId,
                    ProductId = product.Id,
                    Name = product.Name,
                    Type = product.GetType().Name,
                    Quantity = product.Stock
                };

                orderItems.Add(orderItem);
            }

            return orderItems;
        }
    }
}