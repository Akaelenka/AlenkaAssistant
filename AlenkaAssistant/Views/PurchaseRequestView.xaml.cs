using System;
using System.Windows.Controls;
using AlenkaAssistant.ViewModels;
using AlenkaAssistant.Services;
using AlenkaAssistant.Models;

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
            var viewModel = new PurchaseRequestViewModel();
            this.DataContext = viewModel;

            // Load last saved data when view is loaded
            this.Loaded += async (s, e) => await LoadLastSavedDataAsync(viewModel);
        }

        private async System.Threading.Tasks.Task LoadLastSavedDataAsync(PurchaseRequestViewModel viewModel)
        {
            try
            {
                var localDataService = new LocalDataService();
                var lastSavedRequest = await localDataService.LoadLastPurchaseRequestAsync();

                if (lastSavedRequest != null)
                {
                    // Populate form with last saved data
                    viewModel.Uid = lastSavedRequest.UserId ?? "";
                    viewModel.TreatmentDescription = lastSavedRequest.GeneralTreatmentDesc ?? "";
                    viewModel.SelectedTreatmentType = lastSavedRequest.TreatmentType;
                    viewModel.SelectedDoctorName = lastSavedRequest.DoctorName.HasValue
                        ? (lastSavedRequest.DoctorName == DoctorName.Other 
                            ? (lastSavedRequest.AltDoctorName ?? "Other")
                            : DoctorNameHelper.GetDisplayName(lastSavedRequest.DoctorName.Value))
                        : "Other";
                    viewModel.SelectedDate = lastSavedRequest.CreatedAt.Date;
                    viewModel.SelectedTime = lastSavedRequest.CreatedAt.ToString("HH:mm");
                    viewModel.CustomDoctorName = lastSavedRequest.AltDoctorName ?? "";

                    // Populate assistants list
                    if (lastSavedRequest.AltAssistantName != null)
                    {
                        viewModel.AssistantsList.Clear();
                        foreach (var assistantName in lastSavedRequest.AltAssistantName)
                        {
                            var assistantModel = new AssistantModel
                            {
                                SelectedAssistantName = assistantName == "Other" || !viewModel.AssistantNamesForList.Contains(assistantName) 
                                    ? "Other" 
                                    : assistantName,
                                CustomAssistantName = assistantName == "Other" || !viewModel.AssistantNamesForList.Contains(assistantName) 
                                    ? assistantName 
                                    : null
                            };
                            viewModel.AssistantsList.Add(assistantModel);
                        }
                    }

                    // Populate costs list
                    if (lastSavedRequest.CostDetails != null && lastSavedRequest.CostDetails.Count > 0)
                    {
                        viewModel.CostsList.Clear();
                        foreach (var cost in lastSavedRequest.CostDetails)
                        {
                            viewModel.CostsList.Add(new CostModel 
                            { 
                                TreatmentDesc = cost.TreatmentDesc, 
                                Cost = cost.Cost 
                            });
                        }
                    }

                    viewModel.StatusMessage = "✓ Last saved data loaded.";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading last saved data: {ex.Message}");
                // Don't show error to user, just proceed with empty form
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
