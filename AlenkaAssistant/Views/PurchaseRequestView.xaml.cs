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
                        System.Diagnostics.Debug.WriteLine("[View] Uid TextBox LostFocus triggered");
                        // The lookup will be triggered automatically by the ViewModel's Uid property setter
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

                    viewModel.SelectedTreatmentType = lastSavedRequest.TreatmentType;

                    // Fix: Properly load doctor name - ensure it's in the dropdown or set to Other
                    if (lastSavedRequest.DoctorName.HasValue)
                    {
                        string displayName = DoctorNameHelper.GetDisplayName(lastSavedRequest.DoctorName.Value);

                        // Check if the display name is in the available doctor names
                        if (viewModel.DoctorNames != null && viewModel.DoctorNames.Contains(displayName))
                        {
                            viewModel.SelectedDoctorName = displayName;
                            viewModel.CustomDoctorName = "";
                        }
                        else if (lastSavedRequest.DoctorName == DoctorName.Other && !string.IsNullOrWhiteSpace(lastSavedRequest.AltDoctorName))
                        {
                            // Use the custom doctor name if it was saved as Other
                            viewModel.SelectedDoctorName = "Other";
                            viewModel.CustomDoctorName = lastSavedRequest.AltDoctorName;
                        }
                        else
                        {
                            // Fallback: try the first doctor in the list
                            if (viewModel.DoctorNames != null && viewModel.DoctorNames.Count > 0)
                            {
                                viewModel.SelectedDoctorName = viewModel.DoctorNames[0];
                                viewModel.CustomDoctorName = "";
                            }
                        }
                    }
                    else
                    {
                        viewModel.SelectedDoctorName = "Other";
                        viewModel.CustomDoctorName = "";
                    }

                    viewModel.SelectedDate = lastSavedRequest.CreatedAt.Date;
                    viewModel.SelectedTime = lastSavedRequest.CreatedAt.ToString("HH:mm");

                    // Populate assistants list
                    if (lastSavedRequest.AltAssistantName != null)
                    {
                        viewModel.AssistantsList.Clear();
                        foreach (var assistantName in lastSavedRequest.AltAssistantName)
                        {
                            // Check if this name is in the available dropdown list
                            bool isInList = viewModel.AssistantNamesForList != null && viewModel.AssistantNamesForList.Contains(assistantName);

                            var assistantModel = new AssistantModel
                            {
                                // If name is in the dropdown list, use it directly; otherwise mark as "Other"
                                SelectedAssistantName = isInList ? assistantName : "Other",
                                // If not in list, store the custom name; otherwise null
                                CustomAssistantName = isInList ? null : assistantName
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
                                Cost = cost.Cost,
                                TreatmentType = cost.TreatmentType,
                                RM = cost.RM,
                                Month = cost.Month,
                                Discount = cost.Discount
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
