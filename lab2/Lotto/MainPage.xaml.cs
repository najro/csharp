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

        private void ButtonStartLotto_OnClick(object sender, RoutedEventArgs e)
        {
            ButtonStartLotto.IsEnabled = false;

            var numberOfDraws = int.Parse(TextBoxDrawsNo.Text);

            var sumWin5 = 0;
            var sumWin6 = 0;
            var sumWin7 = 0;

            TextCorrectFive.Text = "";
            TextCorrectSix.Text = "";
            TextCorrectSeven.Text = "";

            while (numberOfDraws-- > 0)
            {
                var newDraw = ExecuteNewDraw();

                var correctUserNumbers = _lottoUserInput.Intersect(newDraw).Count();

                switch (correctUserNumbers)
                {
                    case 5:
                        sumWin5++;
                        break;
                    case 6:
                        sumWin6++;
                        break;
                    case 7:
                        sumWin6++;
                        break;
                }

            }

            TextCorrectFive.Text = sumWin5.ToString();
            TextCorrectSix.Text = sumWin6.ToString();
            TextCorrectSeven.Text = sumWin7.ToString();

            ButtonStartLotto.IsEnabled = true;
        }

        private int[] ExecuteNewDraw()
        {
            var lottoNumberSet = new HashSet<int>();

            while (lottoNumberSet.Count < 7)
            {
                // https://learn.microsoft.com/en-us/dotnet/api/system.random.-ctor?view=net-8.0
                var rnd = new Random();
                var number = rnd.Next(1, 35);

                if (!lottoNumberSet.Contains(number))
                {
                    lottoNumberSet.Add(number);
                }
            }

            Debug.WriteLine("Slump : " + lottoNumberSet.ToString());

           // convert set to array with building function
            return lottoNumberSet.ToArray();
            
        }

        //https://learn.microsoft.com/en-us/windows/apps/design/layout/grid-tutorial
    }
    
}
