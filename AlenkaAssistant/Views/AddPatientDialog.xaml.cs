using System.Windows;

namespace AlenkaAssistant.Views
{
    /// <summary>
    /// Interaction logic for AddPatientDialog.xaml
    /// </summary>
    public partial class AddPatientDialog : Window
    {
        public AddPatientDialog()
        {
            InitializeComponent();
        }

        public string RmNumber { get; set; }
        public string PatientName { get; set; }
        public bool Confirmed { get; set; }
    }
}
