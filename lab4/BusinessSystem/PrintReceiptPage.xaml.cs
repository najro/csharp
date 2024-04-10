using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BusinessSystem
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PrintReceiptPage : Page
    {
        public RichTextBlock TextContentBlock { get; set; }

        public PrintReceiptPage()
        {
            this.InitializeComponent();
        }

        public void SetRecieptInfo(string text)
        {
            ReceiptInfo.Text = text;
        }
    }
}
