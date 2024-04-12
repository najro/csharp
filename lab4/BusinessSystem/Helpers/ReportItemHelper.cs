using BusinessSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessSystem.Helpers
{
    // Helper class for report items
    public static class ReportItemHelper
    {
        // Create a string output based on the report GetTop10MostSoldProductsPerYearAndMonth and adjust the output to be more readable
        public static string GetTop10MostSoldProductsPerYearAndMonthReport(List<OrderItem> orderItems)
        {
            var reports = GetTop10MostSoldProductsPerYearAndMonth(orderItems);

            var output = new StringBuilder();

            var YearAndMounthPrefix = string.Empty;

            foreach (var report in reports)
            {
                var currentYearAndMonthPrefix = $"År: {report.Year}, Månad: {GetMonthFriendlyName(report.Month)}";

                if (YearAndMounthPrefix != currentYearAndMonthPrefix)
                {
                    YearAndMounthPrefix = currentYearAndMonthPrefix;
                    output.Append($"\n{YearAndMounthPrefix}\n");
                    output.Append("------------------------------------------------------\n");
                }

                output.Append($"{report.Name}, Antal: {report.Quantity}\n");

            }

            return output.ToString();
        }

        // Get sold products per year and month
        private static List<ReportItem> GetTop10MostSoldProductsPerYearAndMonth(List<OrderItem> orderItems)
        {
            var reports = new List<ReportItem>();

            if (orderItems == null)
            {
                return reports;
            }

            // order this by year, month and name
            var soldProducts = orderItems
                .GroupBy(x => new { x.OrderDate.Year, x.OrderDate.Month, x.Name })
                .Select(x => new
                {
                    Year = x.Key.Year,
                    Month = x.Key.Month,
                    Name = x.Key.Name,
                    Quantity = x.Sum(y => y.Quantity)
                });

    
            var products = soldProducts.ToList();

            // order this by year, month and quantity
            var sortedProducts = products
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Month)
                .ThenByDescending(x => x.Quantity)
                .ToList();


            // take the top 10 for each year and month
            var top10SoldProducts = sortedProducts
                .GroupBy(x => new { x.Year, x.Month })
                .SelectMany(x => x.Take(10));


            // order this by year, month and quantity
            var sortedTop10 = top10SoldProducts.ToList()
                .OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).ThenByDescending(x => x.Quantity).ToList();

            // create a report item for each sold product per year and month
            foreach (var soldProduct in sortedTop10)
            {
                var report = new ReportItem
                {
                    Year = soldProduct.Year,
                    Month = soldProduct.Month,
                    Name = soldProduct.Name,
                    Quantity = soldProduct.Quantity
                };

                reports.Add(report);
            }

            return reports;
        }

        // Get total sales per year and month
        public static string GetTotalSalesPerYearAndMonthReport(List<OrderItem> orderItems)
        {
            var reports = GetTotalSalesPerYearAndMonth(orderItems);

            var output = new StringBuilder();

            var YearAndMounthPrefix = string.Empty;

            foreach (var report in reports)
            {
                var currentYearAndMonthPrefix = $"År: {report.Year}, Månad: {GetMonthFriendlyName(report.Month)}";

                if (YearAndMounthPrefix != currentYearAndMonthPrefix)
                {
                    YearAndMounthPrefix = currentYearAndMonthPrefix;
                    output.Append($"\n{YearAndMounthPrefix}\n");
                    output.Append("-----------------------------------\n");
                }

                output.Append($"Total försäljning: {report.TotalSales}\n");

            }

            return output.ToString();
        }

        // Create GetTotalSalesPerYearAndMonth
        private static List<ReportItem> GetTotalSalesPerYearAndMonth(List<OrderItem> orderItems)
        {
            var reports = new List<ReportItem>();

            if (orderItems == null)
            {
                return reports;
            }

            var totalSalesPerYearAndMonth = orderItems
                .GroupBy(x => new { x.OrderDate.Year, x.OrderDate.Month })
                .Select(x => new
                {
                    Year = x.Key.Year,
                    Month = x.Key.Month,
                    TotalSales = x.Sum(y => y.Quantity)
                });

            var sortedTotalSales = totalSalesPerYearAndMonth
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Month)
                .ToList();

           

            foreach (var totalSales in sortedTotalSales)
            {
                var report = new ReportItem
                {
                    Year = totalSales.Year,
                    Month = totalSales.Month,
                    TotalSales = totalSales.TotalSales
                };

                reports.Add(report);
            }

            return reports;
        }





        // Get the friendly name of the month
        private static string GetMonthFriendlyName(int month)
        {
            switch (month)
            {
                case 1:
                    return "Januari";
                case 2:
                    return "Februari";
                case 3:
                    return "Mars";
                case 4:
                    return "April";
                case 5:
                    return "Maj";
                case 6:
                    return "Juni";
                case 7:
                    return "Juli";
                case 8:
                    return "Augusti";
                case 9:
                    return "September";
                case 10:
                    return "Oktober";
                case 11:
                    return "November";
                case 12:
                    return "December";
                default:
                    return string.Empty;
            }
        }

    }
}
