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
                    output.Append($"{YearAndMounthPrefix}\n");
                }

                output.Append($"{report.Name}, Antal: {report.Quantity}\n");

            }

            return output.ToString();
        }

        // Get the top 10 most sold products per year and month
        private static List<ReportItem> GetTop10MostSoldProductsPerYearAndMonth(List<OrderItem> orderItems)
        {
            var reports = new List<ReportItem>();

            if (orderItems == null)
            {
                return reports;
            }

            var top10MostSoldProducts = orderItems
                .GroupBy(x => new { x.OrderDate.Year, x.OrderDate.Month, x.Name })
                .OrderByDescending(x => x.Sum(y => y.Quantity))
                .Select(x => new
                {
                    Year = x.Key.Year,
                    Month = x.Key.Month,
                    Name = x.Key.Name,
                    Quantity = x.Sum(y => y.Quantity)
                })
                .Take(10); // take the top 10 most sold products

            foreach (var top10MostSoldProduct in top10MostSoldProducts)
            {
                var report = new ReportItem
                {
                    Year = top10MostSoldProduct.Year,
                    Month = top10MostSoldProduct.Month,
                    Name = top10MostSoldProduct.Name,
                    Quantity = top10MostSoldProduct.Quantity
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
                    output.Append($"{YearAndMounthPrefix}\n");
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
                    TotalSales = x.Sum(y => y.Price * y.Quantity)
                });

            foreach (var totalSales in totalSalesPerYearAndMonth)
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
