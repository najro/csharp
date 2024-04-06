using BusinessSystem.Models;
using BusinessSystem.Models.Enums;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace BusinessSystem.Extensions
{
    /// <summary>
    /// Extension methods for TextBox
    /// </summary>
    public static class TextBoxExtensions
    {
        /// <summary>
        /// Display validation color
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="isValid"></param>
        public static void DisplayValidationColor(this TextBox textBox, bool isValid)
        {
            textBox.BorderBrush = isValid ? new SolidColorBrush(Windows.UI.Colors.Green) : new SolidColorBrush(Windows.UI.Colors.Red);
        }


        public static bool IsValidProductId(this TextBox textBox, string textInput, List<Product> existingProducts, Product selectedProduct)
        {
            // verify that input is valid
            if (!int.TryParse(textInput, out int inputValue) || (inputValue < 0 || inputValue >= int.MaxValue))
            {
                return false;
            }

            if (selectedProduct != null && selectedProduct.Id == inputValue)
            {
                return true;
            } 

            if(existingProducts.Exists(p => p.Id == inputValue))
            {
                return false;
            }

            return true;

        }

        public static bool IsValidProductName(this TextBox textBox, string textInput)
        {
            // verify that input is valid
            if (string.IsNullOrWhiteSpace(textInput))
            {
                return false;
            }

            return true;

        }

        public static bool IsValidProductPrice(this TextBox textBox, string textInput)
        {
            // verify that input is valid
            if (!double.TryParse(textInput, out double inputValue) || (inputValue < 0 || inputValue >= double.MaxValue))
            {
                return false;
            }

            return true;

        }

        public static bool IsValidProductStock(this TextBox textBox, string textInput)
        {
            // verify that input is valid
            if (!double.TryParse(textInput, out double inputValue) || (inputValue < 0 || inputValue >= double.MaxValue))
            {
                return false;
            }

            return true;

        }

        public static bool IsValidProductPlaytime(this TextBox textBox, string textInput)
        {
            // verify that input is valid
            if (!string.IsNullOrWhiteSpace(textInput) && ( !double.TryParse(textInput, out double inputValue) || (inputValue <= 0 || inputValue >= double.MaxValue)))
            {
                return false;
            }

            return true;

        }

    }
}
