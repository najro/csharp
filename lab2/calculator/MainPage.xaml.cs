using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace calculator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private int? previousResult = null;
        private string currentOperation = string.Empty;
        private bool newNumberInput = true;
        private bool hasCalculated = false;

        public MainPage()
        {
            this.InitializeComponent();
            CalculateClear();
        }

        private void CalculateClear()
        {
            previousResult = null;
            TextBlockNumberInput.Text = "0";
        }

        private void ButtonNumber_OnClick(object sender, RoutedEventArgs e)
        {

            if (newNumberInput)
            {
                TextBlockNumberInput.Text = "0";
                newNumberInput = false;
            }

            if (hasCalculated)
                previousResult = null;

            var textBox = sender as Button;
            var number = textBox?.Content;

            if (TextBlockNumberInput.Text.StartsWith("0"))
            {
                TextBlockNumberInput.Text = TextBlockNumberInput.Text.Remove(0);
            }

            TextBlockNumberInput.Text = $"{TextBlockNumberInput.Text}{number}";
        }



        private void Calculate(string operation)
        {
            switch (operation)
            {
                case "+":
                    previousResult += GetCurrentNumberInput();
                    break;
                case "-":
                    previousResult -= GetCurrentNumberInput();
                    break;
                case "/": // handle 00000
                    previousResult /= GetCurrentNumberInput();
                    break;
                case "X":
                    previousResult *= GetCurrentNumberInput();
                    break;

            }
        }

       

        private void ButtonCalculate_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(currentOperation) || previousResult == null)
            {
                return;
            }
            Calculate(currentOperation);
            currentOperation = string.Empty;
            TextBlockNumberInput.Text = previousResult.ToString();
            newNumberInput = true;
            hasCalculated = true;
        }

        private void ButtonOperation_OnClick(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(currentOperation) && previousResult == null && GetCurrentNumberInput() == 0)
            {
                return;
            }
            var textBox = sender as Button;
            var operation = textBox?.Content;
            //passer på at kalkuleringen skjer når all info er på plass
            if (!string.IsNullOrWhiteSpace(currentOperation) && previousResult != null)
            {
                if (newNumberInput)
                {
                    currentOperation = operation?.ToString();
                    return;
                }
                Calculate(currentOperation);
                TextBlockNumberInput.Text = previousResult.ToString();
            }
            

            currentOperation = operation?.ToString();

            if (previousResult == null)
            {
                previousResult = GetCurrentNumberInput();
            }
            hasCalculated = false;
            newNumberInput = true;
        }





        private int GetCurrentNumberInput()
        {
            var inputNumber = int.Parse(TextBlockNumberInput.Text);
            return inputNumber;
        }

        private void ButtonClear_OnClick(object sender, RoutedEventArgs e)
        {
            CalculateClear();
        }

    }
}
