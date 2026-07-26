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

        public PrintPreviewWindow(FlowDocument document, string title = "Print Preview")
        {
            InitializeComponent();
            _document = document;
            Title = title;
            DocumentViewer.Document = document;
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            var printDialog = new System.Windows.Controls.PrintDialog();
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
    }
}

