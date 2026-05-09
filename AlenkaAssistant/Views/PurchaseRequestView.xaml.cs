using System.Windows.Controls;
using AlenkaAssistant.ViewModels;

namespace AlenkaAssistant.Views
{
    /// <summary>
    /// Interaction logic for PurchaseRequestView.xaml
    /// </summary>
    public partial class PurchaseRequestView : UserControl
    {
        public PurchaseRequestView()
        {
            InitializeComponent();
            this.DataContext = new PurchaseRequestViewModel();
        }
    }
}
