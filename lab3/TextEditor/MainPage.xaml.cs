using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
// This is a modified version of the Blank Page item template to create a simple text editor

namespace TextEditor
{
    /// <summary>
    /// This is the main page of the text editor app
    /// Backend code :  (lab3 - alfoande100 / Örjan Andersson)
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private bool _isTextChanged = false; // Hold the state of the text input
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

        private StorageFile CurrentStorageFile = null; // Hold the current file in memory

        public MainPage()
        {
            this.InitializeComponent();
            UpdateMetaDataInfo();
            SetAppTitle(CurrentFileName);
        }

        /// <summary>
        /// Update the app title and metadata when the text is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Update the metadata info
        /// </summary>
        private void UpdateMetaDataInfo()
        {
            var numberOfCharactersIncludingSpace = TextInputBox.Text.Length;
            var numberOfCharactersWithoutSpace = TextInputBox.Text.Replace(" ", "").Length;
            var numberOfWords = TextInputBox.Text.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
            var numberOfLines = TextInputBox.Text.Split(new[] { '\r', '\n' }).Length;

            TextBlockNumberCharactersIncludingSpace.Text = string.Format(MetaDataNumberCharactersIncludingSpace, numberOfCharactersIncludingSpace);
            TextBlockNumberCharactersWithoutSpace.Text = string.Format(MetaDataNumberCharactersWithoutSpace, numberOfCharactersWithoutSpace);
            TextBlockNumberWords.Text = string.Format(MetaDataNumberWords, numberOfWords);
            TextBlockNumberLines.Text = string.Format(MetaDataNumberLines, numberOfLines);
        }

        /// <summary>
        /// Set the app title
        /// </summary>
        /// <param name="title"></param>
        private void SetAppTitle(string title)
        {
            if (_isTextChanged)
            {
                title = $"*{title}";
            }

            // https://learn.microsoft.com/en-us/windows/uwp/ui-input/title-bar
            ApplicationView.GetForCurrentView().Title = title;
        }

        /// <summary>
        /// Clear the text input and reset the state to clean start
        /// </summary>
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

        /// <summary>
        ///  Handle the open file button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AppBarButtonOpen_OnClick(object sender, RoutedEventArgs e)
        {
            if (_isTextChanged)
            {
                // https://docs.microsoft.com/en-us/windows/uwp/design/controls-and-patterns/dialogs-and-flyouts/dialogs
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
                    // if the user clicks yes, save the file and then open a new file
                    await SaveFile();
                    OpenFile();
                };

                dialog.SecondaryButtonClick += (s, args) =>
                {
                    // if the user clicks no, open a new file
                    OpenFile();
                };

                await dialog.ShowAsync();
            }
            else
            {
                OpenFile();
            }

        }

        /// <summary>
        /// Handle the save button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AppBarButtonSave_OnClick(object sender, RoutedEventArgs e)
        {
            await SaveFile();
        }
        /// <summary>
        /// Handle the save as button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AppBarButtonSaveAs_OnClick(object sender, RoutedEventArgs e)
        {
            SaveFile(forceFilePicker: true);
        }

        /// <summary>
        /// Handle the new button click. If the text is changed, ask the user if they want to save the file before creating a new file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AppBarButtonNew_OnClick(object sender, RoutedEventArgs e)
        {

            if (_isTextChanged)
            {
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
                    // if the user clicks yes, save the file and then start over with a new text input and file
                    await SaveFile();
                    ClearTextInput();
                };

                dialog.SecondaryButtonClick += (s, args) =>
                {
                    // if the user clicks no, startover with a new text input and file
                    ClearTextInput();
                };

                await dialog.ShowAsync();
            }
            else
            {
                ClearTextInput();
            }
        }

        /// <summary>
        /// Handle the close button click. If the text is changed, ask the user if they want to save the file before closing the app.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AppBarButtonClose_OnClick(object sender, RoutedEventArgs e)
        {
            if (_isTextChanged)
            {
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
                    // if the user clicks yes, save the file and then close the app
                    await SaveFile();
                    CoreApplication.Exit();
                };

                dialog.SecondaryButtonClick += (s, args) =>
                {
                    // if the user clicks no, close the app
                    CoreApplication.Exit();
                };

                await dialog.ShowAsync();
            }
            else
            {
                // https://learn.microsoft.com/en-us/uwp/api/windows.applicationmodel.core.coreapplication.exit?view=winrt-22621
                CoreApplication.Exit();
            }
        }

        /// <summary>
        /// Open a file and set the text input to the content of the file. Hold the file in memory for later use. Update the metadata info.
        /// </summary>
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

        /// <summary>
        /// Save the file. If the file is already saved, save it. If not, open a file picker and save it.
        /// </summary>
        /// <param name="forceFilePicker"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Handle the drop event when the user drops a file into the text input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TextInputBox_OnDrop(object sender, DragEventArgs e)
        {
            // https://mzikmund.dev/blog/the-right-way-to-check-for-key-state-in-uwp-apps
            // https://learn.microsoft.com/en-us/uwp/api/windows.ui.core.corevirtualkeystates?view=winrt-22621
            var ctrlPressed =  Window.Current.CoreWindow.GetKeyState(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down);
            var shiftPressed = Window.Current.CoreWindow.GetKeyState(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down);
            var textFromFile = string.Empty;

            // https://learn.microsoft.com/en-us/windows/apps/design/input/drag-and-drop
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();

                if (items.Count > 0)
                {
                    var storageFile = items.First() as StorageFile;

                    if (storageFile != null && storageFile.FileType == TextExtension)
                    {
                        textFromFile = await FileIO.ReadTextAsync(storageFile);

                        if (ctrlPressed)
                        {
                            TextInputBox.Text += textFromFile;
                        }
                        else if (shiftPressed)
                        {
                            var cursorPosition = TextInputBox.SelectionStart;
                            TextInputBox.Text = TextInputBox.Text.Insert(cursorPosition, textFromFile);
                        }
                        else
                        {
                            ClearTextInput();
                            TextInputBox.Text = textFromFile;
                            UpdateMetaDataInfo();
                            SetAppTitle(CurrentFileName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Change the drag effect to copy when the user drags a file over the text input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextInputBox_OnDragOver(object sender, DragEventArgs e)
        {
            // https://learn.microsoft.com/en-us/windows/apps/design/input/drag-and-drop
            // https://www.c-sharpcorner.com/UploadFile/6d1860/implementing-file-drag-drop-in-your-windows-10-uwp-applica/
            e.AcceptedOperation = DataPackageOperation.Copy;
        }
    }
}