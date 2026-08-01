using System;
using System.Windows;
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

            // Hook up RM LostFocus event to trigger patient lookup
            this.Loaded += (s, e) =>
            {
                if (this.FindName("UidTextBox") is TextBox uidTextBox)
                {
                    uidTextBox.LostFocus += (sender, eventArgs) =>
                    {
                        viewModel.LookupPatientByUid();
                    };
                }

                // Subscribe to ViewModel property changes to update patient display visibility
                viewModel.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(PurchaseRequestViewModel.IsPatientLookupLoading))
                    {
                        UpdatePatientDisplayVisibility(viewModel);
                    }
                    else if (e.PropertyName == nameof(PurchaseRequestViewModel.PatientName) 
                             || e.PropertyName == nameof(PurchaseRequestViewModel.PatientLookupMessage))
                    {
                        UpdatePatientDisplayVisibility(viewModel);
                    }
                };
            };
        }

        private void UpdatePatientDisplayVisibility(PurchaseRequestViewModel viewModel)
        {
            try
            {
                var loadingIndicator = this.FindName("PatientLoadingIndicator") as TextBlock;
                var patientNameDisplay = this.FindName("PatientNameDisplay") as TextBlock;
                var patientErrorDisplay = this.FindName("PatientErrorDisplay") as TextBlock;

                if (loadingIndicator != null)
                {
                    loadingIndicator.Visibility = viewModel.IsPatientLookupLoading ? Visibility.Visible : Visibility.Collapsed;
                }

                if (patientNameDisplay != null)
                {
                    patientNameDisplay.Visibility = !string.IsNullOrWhiteSpace(viewModel.PatientName) ? Visibility.Visible : Visibility.Collapsed;
                }

                if (patientErrorDisplay != null)
                {
                    patientErrorDisplay.Visibility = !string.IsNullOrWhiteSpace(viewModel.PatientLookupMessage) ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[View] Error updating patient display visibility: {ex.Message}");
            }
        }

        private async System.Threading.Tasks.Task LoadLastSavedDataAsync(PurchaseRequestViewModel viewModel)
        {
            try
            {
                // Wait a moment for the ViewModel to initialize dropdowns
                await System.Threading.Tasks.Task.Delay(500);

                var localDataService = new LocalDataService();
                var lastSavedRequest = await localDataService.LoadLastPurchaseRequestAsync();

                if (lastSavedRequest != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[LoadData] Loaded UserId: '{lastSavedRequest.UserId}'");

                    // Populate form with last saved data
                    viewModel.Uid = lastSavedRequest.UserId ?? "";
                    System.Diagnostics.Debug.WriteLine($"[LoadData] Set Uid to: '{viewModel.Uid}'");
                    viewModel.LookupPatientByUid();

                    viewModel.SelectedTreatmentType = lastSavedRequest.TreatmentType;

                    // Load doctor name - now string-based from config
                    if (!string.IsNullOrWhiteSpace(lastSavedRequest.DoctorName))
                    {
                        // Check if the doctor name is in the available list
                        if (viewModel.DoctorNames != null && viewModel.DoctorNames.Contains(lastSavedRequest.DoctorName))
                        {
                            viewModel.SelectedDoctorName = lastSavedRequest.DoctorName;
                        }
                        else if (viewModel.DoctorNames != null && viewModel.DoctorNames.Count > 0)
                        {
                            // Fallback to first in list
                            viewModel.SelectedDoctorName = viewModel.DoctorNames[0];
                        }
                    }
                    else
                    {
                        if (viewModel.DoctorNames != null && viewModel.DoctorNames.Count > 0)
                        {
                            viewModel.SelectedDoctorName = viewModel.DoctorNames[0];
                        }
                    }

                    // Load assistant names - now using AssistantsList with config-driven dropdown
                    if (lastSavedRequest.AssistantNames != null && lastSavedRequest.AssistantNames.Count > 0)
                    {
                        viewModel.AssistantsList.Clear();
                        foreach (var assistantName in lastSavedRequest.AssistantNames)
                        {
                            var assistantItem = new AssistantItem();

                            // Check if the name is in the available list
                            if (viewModel.AssistantNamesForList != null && viewModel.AssistantNamesForList.Contains(assistantName))
                            {
                                assistantItem.SelectedAssistantName = assistantName;
                                assistantItem.ShowCustomInput = false;
                            }
                            else
                            {
                                // Must be a custom name, set as "Other"
                                assistantItem.SelectedAssistantName = "Other";
                                assistantItem.CustomAssistantName = assistantName;
                                assistantItem.ShowCustomInput = true;
                            }

                            viewModel.AssistantsList.Add(assistantItem);
                        }
                    }
                    else
                    {
                        viewModel.AssistantsList.Clear();
                    }

                    viewModel.SelectedDate = lastSavedRequest.CreatedAt.Date;
                    viewModel.SelectedTime = lastSavedRequest.CreatedAt.ToString("HH:mm");

                    // Populate costs list
                    if (lastSavedRequest.CostDetails != null && lastSavedRequest.CostDetails.Count > 0)
                    {
                        viewModel.CostsList.Clear();
                        foreach (var cost in lastSavedRequest.CostDetails)
                        {
                            viewModel.CostsList.Add(cost);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LoadData] Error loading last saved data: {ex.Message}");
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
