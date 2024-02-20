using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Lotto
{
    /// <summary>
    /// Backend code for lotto GUI  (lab2 - alfoande100 / Örjan Andersson)
    ///
    /// Grid is inspired from https://learn.microsoft.com/en-us/windows/apps/design/layout/grid-tutorial
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // deafult set all values to 0
        private int[] _lottoUserInput = { 0, 0, 0, 0, 0, 0, 0 };

        // string constants for displaying number of wins
        private const string WinInfoFive = "5 rätt: {0}";
        private const string WinInfoSix = "6 rätt: {0}";
        private const string WinInfoSeven = "7 rätt: {0}";

        public MainPage()
        {
            this.InitializeComponent();
            SetWinnerInfo();
        }

        /// <summary>
        /// Helper function to check if all input is valid and enable lotto start button
        /// </summary>
        private void CheckValidLottoInput()
        {
            ButtonStartLotto.IsEnabled = !_lottoUserInput.Contains(0) && int.TryParse(TextBoxDrawsNo.Text, out int result);
        }

        #region Events

        /// <summary>
        /// Check numbers of input draws to play lotto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxDrawsNo_OnKeyUp(object sender, KeyRoutedEventArgs e)
        {

            var textBox = sender as TextBox;

            var textInput = textBox?.Text;
            var textBoxName = textBox?.Name;

            // verify that input is valid
            if (!int.TryParse(textInput, out int inputValue) || (inputValue < 1 || inputValue > 999999))
            {
                textBox.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
            }
            else
            {
                // valid value
                textBox.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Green);
            }

            // check if all input is valid
            CheckValidLottoInput();
        }

        /// <summary>
        /// Verify that input is valid for each users number and enable lotto start button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxLotto_OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            var textBox = sender as TextBox;

            var textInput = textBox?.Text;
            var textBoxName = textBox?.Name;


            // always clear input value before verifying any new input
            _lottoUserInput[GetTextBoxLottoInputPosition(textBoxName)] = 0;

            // verify that input is valid
            if (!int.TryParse(textInput, out int inputValue) || (inputValue < 1 || inputValue > 35) || _lottoUserInput.Contains(inputValue))
            {
                textBox.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
            }
            else
            {
                // valid value
                textBox.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Green);
                _lottoUserInput[GetTextBoxLottoInputPosition(textBoxName)] = inputValue;
            }

            // check if all input is valid
            CheckValidLottoInput();
        }

        /// <summary>
        /// Perform lotto draw and calculate number of wins
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonStartLotto_OnClick(object sender, RoutedEventArgs e)
        {
            ButtonStartLotto.IsEnabled = false;

            var numberOfDraws = int.Parse(TextBoxDrawsNo.Text);

            var winFiveCount = 0;
            var winSixCount = 0;
            var winSevenCount = 0;

            while (numberOfDraws-- > 0)
            {
                var newDraw = ExecuteNewDraw();

                var correctUserNumbers = _lottoUserInput.Intersect(newDraw).Count();

                switch (correctUserNumbers)
                {
                    case 5:
                        winFiveCount++;
                        break;
                    case 6:
                        winSixCount++;
                        break;
                    case 7:
                        winSevenCount++;
                        break;
                }
            }

            SetWinnerInfo(winFiveCount, winSixCount, winSevenCount);
            ButtonStartLotto.IsEnabled = true;
        }

        #endregion


        #region Helper functions
        /// <summary>
        /// Display number of wins
        /// </summary>
        /// <param name="winFiveCount"></param>
        /// <param name="winSixCount"></param>
        /// <param name="winSevenCount"></param>
        private void SetWinnerInfo(int winFiveCount = 0, int winSixCount = 0, int winSevenCount = 0)
        {
            TextBlockFiveInfo.Text = string.Format(WinInfoFive, winFiveCount);
            TextBlockSixInfo.Text = string.Format(WinInfoSix, winSixCount);
            TextBlockSevenInfo.Text = string.Format(WinInfoSeven, winSevenCount);
        }

        /// <summary>
        /// Get position of input of users lotto numbers in array
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>

        private int GetTextBoxLottoInputPosition(string name)
        {
            var position = name.Replace("TextBoxLotto", "");
            return int.Parse(position) - 1;
        }

        /// <summary>
        /// Execute new draw and return array of lotto numbers for new draw
        /// </summary>
        /// <returns></returns>
        private int[] ExecuteNewDraw()
        {
            var lottoNumberSet = new HashSet<int>();

            while (lottoNumberSet.Count < 7)
            {
                // https://learn.microsoft.com/en-us/dotnet/api/system.random.-ctor?view=net-8.0
                var rnd = new Random();
                var number = rnd.Next(1, 36);

                if (!lottoNumberSet.Contains(number))
                {
                    lottoNumberSet.Add(number);
                }
            }

            // convert set to array with building function
            return lottoNumberSet.ToArray();

        }
        #endregion
    }
}
