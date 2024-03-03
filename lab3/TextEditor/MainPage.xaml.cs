using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core.Preview;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TextEditor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private bool _isTextChanged = false;
        private bool _openNewOrStartOver = true;
        private const string TextExtension = ".txt";
        private const string DefaultFileName = "dok1.txt";
        private string CurrentFileName = DefaultFileName;

        private const string MetaDataNumberCharactersIncludingSpace = "Tecken med mellanslag: {0}";
        private const string MetaDataNumberCharactersWithoutSpace = "Tecken utan mellanslag: {0}";
        private const string MetaDataNumberWords = "Antal ord: {0}";
        private const string MetaDataNumberLines = "Antal rader: {0}";

        private const string UnsavedChangesTitle = "Osparade ändringar";
        private const string UnsavedChangesContent = "Vill du lagra dina ändringar?";
        private const string UnsavedChangesPrimaryButtonTextYes = "Ja";
        private const string UnsavedChangesSecondaryButtonTextNo = "Nej";
        private const string UnsavedChangesCloseButtonTextCancel = "Avbryt";


        private StorageFile CurrentStorageFile = null;

        public MainPage()
        {
            this.InitializeComponent();
            UpdateMetaDataInfo();
            SetAppTitle(CurrentFileName);
        }

        private void TextInputBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_openNewOrStartOver)
            {
                _isTextChanged = false;
                _openNewOrStartOver = false;
            }
            else if (TextInputBox.Text.Length == 0 && CurrentFileName == DefaultFileName)
            {
                _isTextChanged = false;
            }
            else
            {
                _isTextChanged = true;
            }

            SetAppTitle(CurrentFileName);
            UpdateMetaDataInfo();

        }

        private void UpdateMetaDataInfo()
        {

            // TODO add reference to split documentation from microsoft

            var numberOfCharactersIncludingSpace = TextInputBox.Text.Length;
            var numberOfCharactersWithoutSpace = TextInputBox.Text.Replace(" ", "").Length;
            var numberOfWords = TextInputBox.Text.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
            var numberOfLines = TextInputBox.Text.Split(new[] { '\r', '\n' }).Length;

            TextBlockNumberCharactersIncludingSpace.Text = string.Format(MetaDataNumberCharactersIncludingSpace, numberOfCharactersIncludingSpace);
            TextBlockNumberCharactersWithoutSpace.Text = string.Format(MetaDataNumberCharactersWithoutSpace, numberOfCharactersWithoutSpace);
            TextBlockNumberWords.Text = string.Format(MetaDataNumberWords, numberOfWords);
            TextBlockNumberLines.Text = string.Format(MetaDataNumberLines, numberOfLines);


        }

        private void SetAppTitle(string title)
        {
            if (_isTextChanged)
            {
                title = $"*{title}";
            }

            // https://learn.microsoft.com/en-us/windows/uwp/ui-input/title-bar
            ApplicationView.GetForCurrentView().Title = title;
        }


        private void ClearTextInput()
        {
            TextInputBox.Text = string.Empty;
            _isTextChanged = false;
            _openNewOrStartOver = true;
            CurrentFileName = DefaultFileName;
            CurrentStorageFile = null;

            SetAppTitle(CurrentFileName);
            UpdateMetaDataInfo();
        }


        private async void AppBarButtonOpen_OnClick(object sender, RoutedEventArgs e)
        {


            if (_isTextChanged)
            {
                // add documentation for ContentDialog
                var dialog = new ContentDialog
                {
                    Title = UnsavedChangesTitle,
                    Content = UnsavedChangesContent,
                    PrimaryButtonText = UnsavedChangesPrimaryButtonTextYes,
                    SecondaryButtonText = UnsavedChangesSecondaryButtonTextNo,
                    CloseButtonText = UnsavedChangesCloseButtonTextCancel
                };

                dialog.PrimaryButtonClick += async (s, args) =>
                {
                    await SaveFile();
                    OpenFile();
                };

                dialog.SecondaryButtonClick += (s, args) =>
                {
                     OpenFile();
                };

                await dialog.ShowAsync();
            }
            else
            {
                OpenFile();
            }

        }

        private async void OpenFile()
        {
            var fileOpenPicker = new FileOpenPicker();
            fileOpenPicker.FileTypeFilter.Add(TextExtension);
            var result = await fileOpenPicker.PickSingleFileAsync();

            if (result != null)
            {
                CurrentStorageFile = result;
                CurrentFileName = result.Name;
                var text = await FileIO.ReadTextAsync(result);
                _openNewOrStartOver = true;
                TextInputBox.Text = text;
            }

            UpdateMetaDataInfo();
        }

        private async void AppBarButtonSave_OnClick(object sender, RoutedEventArgs e)
        {
            await SaveFile();
        }

        private async Task SaveFile(bool forceFilePicker = false)
        {
            if (CurrentStorageFile != null && forceFilePicker != true)
            {
                await FileIO.WriteTextAsync(CurrentStorageFile, TextInputBox.Text);
                _isTextChanged = false;
                SetAppTitle(CurrentFileName);
                return;
            }


            var fileSavePicker = new FileSavePicker();
            fileSavePicker.FileTypeChoices.Add("Plain Text", new[] { TextExtension });

            var result = await fileSavePicker.PickSaveFileAsync();

            if (result != null)
            {
                await FileIO.WriteTextAsync(result, TextInputBox.Text);
                CurrentStorageFile = result;
                CurrentFileName = result.Name;
                _isTextChanged = false;
                SetAppTitle(CurrentFileName);
            }
        }

        private async void AppBarButtonSaveAs_OnClick(object sender, RoutedEventArgs e)
        {
            SaveFile(forceFilePicker:true);
            //var fileSavePicker = new FileSavePicker();
            //fileSavePicker.FileTypeChoices.Add("Plain Text", new[] { TextExtension });

            //var result = await fileSavePicker.PickSaveFileAsync();

            //if (result != null)
            //{
            //    await FileIO.WriteTextAsync(result, TextInputBox.Text);
            //    CurrentStorageFile = result;
            //    CurrentFileName = result.Name;
            //    _isTextChanged = false;
            //    SetAppTitle(CurrentFileName);
            //}
        }

        private async void AppBarButtonNew_OnClick(object sender, RoutedEventArgs e)
        {

            if (_isTextChanged)
            {
                // add documentation for ContentDialog
                var dialog = new ContentDialog
                {
                    Title = UnsavedChangesTitle,
                    Content = UnsavedChangesContent,
                    PrimaryButtonText = UnsavedChangesPrimaryButtonTextYes,
                    SecondaryButtonText = UnsavedChangesSecondaryButtonTextNo,
                    CloseButtonText = UnsavedChangesCloseButtonTextCancel
                };

                dialog.PrimaryButtonClick += async (s, args) =>
                {
                    //AppBarButtonSave_OnClick(sender, e);
                    await SaveFile();
                    ClearTextInput();
                };

                dialog.SecondaryButtonClick += (s, args) =>
                {
                    ClearTextInput();
                };

                await dialog.ShowAsync();
            }
            else
            {
                ClearTextInput();
            }
        }

        private async void AppBarButtonClose_OnClick(object sender, RoutedEventArgs e)
        {

            if (_isTextChanged)
            {
                // add documentation for ContentDialog
                var dialog = new ContentDialog
                {
                    Title = UnsavedChangesTitle,
                    Content = UnsavedChangesContent,
                    PrimaryButtonText = UnsavedChangesPrimaryButtonTextYes,
                    SecondaryButtonText = UnsavedChangesSecondaryButtonTextNo,
                    CloseButtonText = UnsavedChangesCloseButtonTextCancel
                };

                dialog.PrimaryButtonClick += async (s, args) =>
                {
                    await SaveFile();
                    CoreApplication.Exit();
                };

                dialog.SecondaryButtonClick += (s, args) =>
                {
                    CoreApplication.Exit();
                };

                await dialog.ShowAsync();
            }
            else
            {
                // https://learn.microsoft.com/en-us/uwp/api/windows.ui.xaml.application.exit?view=winrt-22621
                CoreApplication.Exit();
            }

        }

        private void TextInputBox_OnDrop(object sender, DragEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
