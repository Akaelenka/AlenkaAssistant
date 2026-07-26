using System.Windows;
using System.Windows.Documents;

namespace AlenkaAssistant.Views
{
    /// <summary>
    /// Print preview window for displaying documents before printing
    /// </summary>
    public partial class PrintPreviewWindow : Window
    {
        private FlowDocument _document;
        private readonly string _paperSize;

        public PrintPreviewWindow(FlowDocument document, string title = "Print Preview", string paperSize = "A4")
        {
            InitializeComponent();
            _document = document;
            _paperSize = paperSize;
            Title = title;
            DocumentViewer.Document = document;
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            var printDialog = new System.Windows.Controls.PrintDialog();
            ApplyPaperSizeToPrintDialog(printDialog);

            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintDocument(((System.Windows.Documents.IDocumentPaginatorSource)_document).DocumentPaginator, "Alenka Invoice");
                MessageBox.Show("Document sent to printer!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ApplyPaperSizeToPrintDialog(System.Windows.Controls.PrintDialog printDialog)
        {
            var pageMediaSizeName = _paperSize.ToUpperInvariant() switch
            {
                "A5" => System.Printing.PageMediaSizeName.ISOA5,
                "A4" => System.Printing.PageMediaSizeName.ISOA4,
                "LETTER" => System.Printing.PageMediaSizeName.NorthAmericaLetter,
                "LEGAL" => System.Printing.PageMediaSizeName.NorthAmericaLegal,
                _ => System.Printing.PageMediaSizeName.ISOA4
            };

            printDialog.PrintTicket.PageMediaSize = new System.Printing.PageMediaSize(pageMediaSizeName);
        }
    }
}

