using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace calculator
{
    /// <summary>
    /// Calculator app (lab2 - Örjan Andersson)
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private int? previousResult = null;
        private string currentOperation = string.Empty;
        private bool newNumberInput = true;
        private bool hasCalculated = false;
        private bool errorDialogIsVisible = false;

        public MainPage()
        {
            this.InitializeComponent();
            CalculateClear();
        }

        private void CalculateClear()
        {
            newNumberInput = true;
            currentOperation = string.Empty;
            previousResult = null;
            TextBlockNumberInput.Text = "0";
        }

      
        /// <summary>
        /// reset calculator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClear_OnClick(object sender, RoutedEventArgs e)
        {
            HideErrorMessage();
            CalculateClear();
        }

        /// <summary>
        /// handle click on a number
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonNumber_OnClick(object sender, RoutedEventArgs e)
        {
            HideErrorMessage();

            // start a new input serie
            if (newNumberInput)
            {
                TextBlockNumberInput.Text = "0";
                newNumberInput = false;
            }

            // if = has been used, then a new input serie should reset earlier result and star over
            if (hasCalculated)
            {
                previousResult = null;
            }
            
            // handle number display
            // each button contains number in content property
            var textBox = sender as Button;
            var number = textBox?.Content;

            // do not start to display a number with 0
            if (TextBlockNumberInput.Text.StartsWith("0"))
            {
                TextBlockNumberInput.Text = TextBlockNumberInput.Text.Remove(0);
            }

            TextBlockNumberInput.Text = $"{TextBlockNumberInput.Text}{number}";
        }


        /// <summary>
        /// handle click on = button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCalculate_OnClick(object sender, RoutedEventArgs e)
        {
            HideErrorMessage();

            // if no operation selected or previous result, then do nothing
            if (string.IsNullOrWhiteSpace(currentOperation) || previousResult == null)
            {
                return;
            }

            Calculate(currentOperation);
            currentOperation = string.Empty;
            newNumberInput = true;
            hasCalculated = true;
        }

        private void ButtonOperation_OnClick(object sender, RoutedEventArgs e)
        {
            HideErrorMessage();

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
                //SetCurrentNumberInputDisplay();
            }
            

            currentOperation = operation?.ToString();

            if (previousResult == null)
            {
                previousResult = GetCurrentNumberInput();
            }
            hasCalculated = false;
            newNumberInput = true;
        }

        private void ShowErrorMessage(string message)
        {
            TextBlockErrorInfo.Text = message;
            TextBlockErrorInfo.Visibility = Visibility.Visible;
            errorDialogIsVisible = true;

            // reset calculator and start over
            CalculateClear();
        }

        private void HideErrorMessage()
        {
            if (!errorDialogIsVisible) return;

            TextBlockErrorInfo.Visibility = Visibility.Collapsed;
            errorDialogIsVisible = false;
        }


        private void Calculate(string operation)
        {
            try
            {
                var currentInput = GetCurrentNumberInput();

                switch (operation)
                {
                    case "+":
                        previousResult += currentInput;
                        break;
                    case "-":
                        previousResult -= currentInput;
                        break;
                    case "/": // handle 00000

                        if (currentInput == 0)
                        {
                            ShowErrorMessage("Du kan inte dela med 0!");
                            return;
                        }

                        previousResult /= currentInput;
                        break;
                    case "X":
                        previousResult *= currentInput;
                        break;

                }

                TextBlockNumberInput.Text = previousResult.ToString();
            }
            catch (OverflowException)
            {
                // https://learn.microsoft.com/en-us/dotnet/api/system.overflowexception?view=net-8.0
                ShowErrorMessage("Du jobbar med för stora tal");
            }
            catch
            {
                ShowErrorMessage("Okänt problem har uppstått");
            }
        }


        // get current number input as integer
        private int GetCurrentNumberInput()
        {
            try
            {
                var inputNumber = int.Parse(TextBlockNumberInput.Text);
                return inputNumber;
            }
            catch (OverflowException)
            {
                // https://learn.microsoft.com/en-us/dotnet/api/system.overflowexception?view=net-8.0
                ShowErrorMessage("Du jobbar med för stora tal");
            }
            catch
            {
                ShowErrorMessage("Okänt problem har uppstått");
            }

            return 0;
        }

    }
}
