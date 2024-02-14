using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage.Pickers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TextEditor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private bool _isTextChanged = false;
        private const string TextExtension = ".txt";
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void TextInputBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _isTextChanged = true;
            UpdateMetaDataInfo();
        }

        private void UpdateMetaDataInfo()
        {
            TextBlockMetaData.Text = $"Last updated: {DateTime.Now}";
        }

        private async void AppBarButtonOpen_OnClick(object sender, RoutedEventArgs e)
        {
            var fileOpenPicker = new FileOpenPicker();
            fileOpenPicker.FileTypeFilter.Add(TextExtension);
            var result = await fileOpenPicker.PickSingleFileAsync();

            if (result != null)
            {
                var text = await FileIO.ReadTextAsync(result);
                TextInputBox.Text = text;
            }

            UpdateMetaDataInfo();

        }

        private async void AppBarButtonSave_OnClick(object sender, RoutedEventArgs e)
        {
            var fileSavePicker = new FileSavePicker();
            fileSavePicker.FileTypeChoices.Add("Plain Text", new[] { TextExtension });

            var result = await fileSavePicker.PickSaveFileAsync();

            if (result != null)
            {
                await FileIO.WriteTextAsync(result, TextInputBox.Text);
            }
        }


        private void AppBarButtonNew_OnClick(object sender, RoutedEventArgs e)
        {
            if (_isTextChanged)
            {
                var dialog = new ContentDialog
                {
                    Title = "Unsaved changes",
                    Content = "Do you want to save changes?",
                    PrimaryButtonText = "Yes",
                    SecondaryButtonText = "No",
                    CloseButtonText = "Cancel"
                };

                dialog.PrimaryButtonClick += async (s, args) =>
                {
                    //await AppBarButtonSave_OnClick(sender, e);
                    //TextInputBox.Text = string.Empty;
                };

                dialog.SecondaryButtonClick += (s, args) =>
                {
                    TextInputBox.Text = string.Empty;
                };

                dialog.ShowAsync();
            }
            else
            {
                TextInputBox.Text = string.Empty;
            }

        }
    }
}
