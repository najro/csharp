using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Lotto
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
       
        int[] _lottoUserInput = { 0, 0, 0, 0, 0, 0, 0 };
        //int[] lottoUserGenerate = { 0, 0, 0, 0, 0, 0, 0 };

        public MainPage()
        {
            this.InitializeComponent();
        }

        private int GetTextBoxLottoInputPosition(string name)
        {
            var position = name.Replace("TextBoxLotto", "");
            return int.Parse(position) - 1;
        }


        private void CheckValidLottoInput()
        {
            ButtonStartLotto.IsEnabled = !_lottoUserInput.Contains(0);
        }


        private void TextBoxLotto_OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            var textBox = sender as TextBox;

            var textInput = textBox?.Text;
            var textBoxName = textBox?.Name;


            _lottoUserInput[GetTextBoxLottoInputPosition(textBoxName)] = 0;

            // verify that input is valid
            if (!int.TryParse(textInput, out int inputValue) || 
                (inputValue < 1 || inputValue > 35) ||
                _lottoUserInput.Contains(inputValue))
            {

                textBox.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
            }
            else
            {
                // valid value
                textBox.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Green);
                _lottoUserInput[GetTextBoxLottoInputPosition(textBoxName)] = inputValue;
            }


            CheckValidLottoInput();

            //// Find common elements
            //var commonNumbers = array1.Intersect(array2);

        }

    }
    
}
