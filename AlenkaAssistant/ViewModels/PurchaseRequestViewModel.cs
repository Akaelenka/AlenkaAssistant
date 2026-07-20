using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using AlenkaAssistant.Models;
using AlenkaAssistant.Services;

namespace AlenkaAssistant.ViewModels
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object parameter) => _execute(parameter);
    }

    public class PurchaseRequestViewModel : INotifyPropertyChanged
    {
        private string _uid;
        private string _treatmentDescription;
        private TreatmentType? _selectedTreatmentType;
        private DoctorName? _selectedDoctor;
        private DateTime? _selectedDate;
        private string _selectedTime;
        private string _statusMessage;
        private string _customDoctorName;
        private bool _showCustomDoctor;

        public string Uid
        {
            get => _uid;
            set
            {
                if (_uid != value)
                {
                    _uid = value;
                    OnPropertyChanged(nameof(Uid));
                }
            }
        }

        public string TreatmentDescription
        {
            get => _treatmentDescription;
            set
            {
                if (_treatmentDescription != value)
                {
                    _treatmentDescription = value;
                    OnPropertyChanged(nameof(TreatmentDescription));
                }
            }
        }

        public TreatmentType? SelectedTreatmentType
        {
            get => _selectedTreatmentType;
            set
            {
                if (_selectedTreatmentType != value)
                {
                    _selectedTreatmentType = value;
                    OnPropertyChanged(nameof(SelectedTreatmentType));
                }
            }
        }

        public ObservableCollection<DisplayItem<TreatmentType>> TreatmentTypes { get; }
        public ObservableCollection<DisplayItem<AssistantName>> AssistantNamesForList { get; }
        public ObservableCollection<DisplayItem<DoctorName>> DoctorNames { get; }
        public ObservableCollection<CostModel> CostsList { get; }
        public ObservableCollection<AssistantModel> AssistantsList { get; }

        public DoctorName? SelectedDoctor
        {
            get => _selectedDoctor;
            set
            {
                if (_selectedDoctor != value)
                {
                    _selectedDoctor = value;
                    OnPropertyChanged(nameof(SelectedDoctor));

                    // Update visibility of custom doctor textbox
                    ShowCustomDoctor = value == DoctorName.Other;
                    if (value != DoctorName.Other)
                    {
                        CustomDoctorName = string.Empty;
                    }
                }
            }
        }

        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged(nameof(SelectedDate));
                }
            }
        }

        public string SelectedTime
        {
            get => _selectedTime;
            set
            {
                if (_selectedTime != value)
                {
                    _selectedTime = value;
                    OnPropertyChanged(nameof(SelectedTime));
                }
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged(nameof(StatusMessage));
                }
            }
        }

        public string CustomDoctorName
        {
            get => _customDoctorName;
            set
            {
                if (_customDoctorName != value)
                {
                    _customDoctorName = value;
                    OnPropertyChanged(nameof(CustomDoctorName));
                }
            }
        }

        public bool ShowCustomDoctor
        {
            get => _showCustomDoctor;
            set
            {
                if (_showCustomDoctor != value)
                {
                    _showCustomDoctor = value;
                    OnPropertyChanged(nameof(ShowCustomDoctor));
                }
            }
        }

        public int TotalCost
        {
            get
            {
                int total = 0;
                foreach (var item in CostsList)
                {
                    total += item.Cost;
                }
                return total;
            }
        }

        public ICommand SubmitCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand AddCostCommand { get; }
        public ICommand RemoveCostCommand { get; }
        public ICommand AddAssistantCommand { get; }
        public ICommand RemoveAssistantCommand { get; }
        public ICommand SaveToGoogleSheetsCommand { get; }

        public PurchaseRequestViewModel()
        {
            // Initialize collections using helper methods
            TreatmentTypes = TreatmentTypeHelper.GetDisplayItems();
            AssistantNamesForList = AssistantNameHelper.GetDisplayItemsForList();
            DoctorNames = DoctorNameHelper.GetDisplayItems();

            // Initialize costs list with one default item
            CostsList = new ObservableCollection<CostModel>
            {
                new CostModel { TreatmentDesc = string.Empty, Cost = 0 }
            };

            // Initialize assistants list (empty, user must add)
            AssistantsList = new ObservableCollection<AssistantModel>();

            // Subscribe to collection changes to manage cost item event subscriptions
            CostsList.CollectionChanged += CostsList_CollectionChanged;

            // Subscribe to assistants list collection changes
            AssistantsList.CollectionChanged += AssistantsList_CollectionChanged;

            // Subscribe to initial cost items
            foreach (var item in CostsList)
            {
                item.PropertyChanged += CostItem_PropertyChanged;
            }

            // Set defaults
            SelectedDoctor = DoctorName.DrgNovi;
            SelectedDate = DateTime.Today;
            SelectedTime = DateTime.Now.ToString("HH:mm");

            // Initialize commands
            SubmitCommand = new RelayCommand(_ => SubmitRequest(), _ => CanSubmit());
            CancelCommand = new RelayCommand(_ => CancelRequest());
            AddCostCommand = new RelayCommand(_ => AddCost());
            RemoveCostCommand = new RelayCommand(obj => RemoveCost(obj), obj => CanRemoveCost(obj));
            AddAssistantCommand = new RelayCommand(_ => AddAssistant());
            RemoveAssistantCommand = new RelayCommand(obj => RemoveAssistant(obj));
            SaveToGoogleSheetsCommand = new RelayCommand(_ => SaveToGoogleSheetsAsync(), _ => CanSaveToGoogleSheets());
        }

        private bool CanSubmit()
        {
            // Check basic fields
            if (!(!string.IsNullOrWhiteSpace(Uid) 
                && SelectedDate.HasValue 
                && SelectedDoctor.HasValue))
            {
                return false;
            }

            // Check if custom doctor name is required and provided
            if (SelectedDoctor == DoctorName.Other && string.IsNullOrWhiteSpace(CustomDoctorName))
            {
                return false;
            }

            return true;
        }

        private void SubmitRequest()
        {
            // Validate
            if (!CanSubmit())
            {
                if (SelectedDoctor == DoctorName.Other && string.IsNullOrWhiteSpace(CustomDoctorName))
                {
                    StatusMessage = "✗ Please enter a custom doctor name.";
                }
                else
                {
                    StatusMessage = "✗ Please fill in all required fields.";
                }
                return;
            }

            try
            {
                // Create the model
                var purchaseRequest = new PurchaseRequestModel
                {
                    UserId = Uid,
                    GeneralTreatmentDesc = TreatmentDescription,
                    AssistantName = AssistantsList.Count > 0 ? AssistantsList[0].SelectedAssistant : AssistantName.None,
                    DoctorName = SelectedDoctor.Value,
                    CreatedAt = GetDateTimeFromInputs(),
                    AltAssistantName = new List<string>(),
                    CostDetails = CostsList.ToList()
                };

                // Add assistant names to AltAssistantName list
                foreach (var assistant in AssistantsList)
                {
                    string assistantName = assistant.SelectedAssistant == AssistantName.Other 
                        ? (assistant.CustomAssistantName ?? "")
                        : AssistantNameHelper.GetDisplayName(assistant.SelectedAssistant);
                    purchaseRequest.AltAssistantName.Add(assistantName);
                }

                // TODO: Save to database or API
                StatusMessage = $"✓ Purchase request submitted successfully for UID: {Uid}";

                // Clear form after successful submission
                ClearForm();
            }
            catch (Exception ex)
            {
                StatusMessage = $"✗ Error: {ex.Message}";
            }
        }

        private void CancelRequest()
        {
            ClearForm();
            StatusMessage = "Form cleared.";
        }

        private DateTime GetDateTimeFromInputs()
        {
            try
            {
                DateTime selectedDate = SelectedDate ?? DateTime.Today;
                string[] timeParts = (SelectedTime ?? "00:00").Split(':');

                if (timeParts.Length == 2 && int.TryParse(timeParts[0], out int hours) && int.TryParse(timeParts[1], out int minutes))
                {
                    return selectedDate.Add(new TimeSpan(hours, minutes, 0));
                }

                return selectedDate;
            }
            catch
            {
                return DateTime.Now;
            }
        }

        private void ClearForm()
        {
            Uid = string.Empty;
            TreatmentDescription = string.Empty;
            SelectedDoctor = null;
            SelectedDate = DateTime.Today;
            SelectedTime = DateTime.Now.ToString("HH:mm");

            // Clear assistants list
            AssistantsList.Clear();

            // Reset costs list
            CostsList.Clear();
            CostsList.Add(new CostModel { TreatmentDesc = string.Empty, Cost = 0 });
        }

        private void AddCost()
        {
            CostsList.Add(new CostModel { TreatmentDesc = string.Empty, Cost = 0 });
            OnPropertyChanged(nameof(TotalCost));
        }

        private bool CanRemoveCost(object parameter)
        {
            return CostsList.Count > 1;
        }

        private void RemoveCost(object parameter)
        {
            if (parameter is CostModel costItem && CostsList.Count > 1)
            {
                costItem.PropertyChanged -= CostItem_PropertyChanged;
                CostsList.Remove(costItem);
                OnPropertyChanged(nameof(TotalCost));
            }
        }

        private void CostsList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // Unsubscribe from removed items
            if (e.OldItems != null)
            {
                foreach (CostModel item in e.OldItems)
                {
                    item.PropertyChanged -= CostItem_PropertyChanged;
                }
            }

            // Subscribe to added items
            if (e.NewItems != null)
            {
                foreach (CostModel item in e.NewItems)
                {
                    item.PropertyChanged += CostItem_PropertyChanged;
                }
            }

            // Update total cost when collection changes
            OnPropertyChanged(nameof(TotalCost));
        }

        private void CostItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Update total cost whenever a cost item's Cost or TreatmentDesc property changes
            if (e.PropertyName == nameof(CostModel.Cost) || e.PropertyName == nameof(CostModel.TreatmentDesc))
            {
                OnPropertyChanged(nameof(TotalCost));
            }
        }

        private void AddAssistant()
        {
            AssistantsList.Add(new AssistantModel { SelectedAssistant = AssistantName.Pavela, CustomAssistantName = string.Empty });
        }

        private void RemoveAssistant(object parameter)
        {
            if (parameter is AssistantModel assistantItem)
            {
                assistantItem.PropertyChanged -= AssistantItem_PropertyChanged;
                AssistantsList.Remove(assistantItem);
            }
        }

        private void AssistantsList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // Unsubscribe from removed items
            if (e.OldItems != null)
            {
                foreach (AssistantModel item in e.OldItems)
                {
                    item.PropertyChanged -= AssistantItem_PropertyChanged;
                }
            }

            // Subscribe to added items
            if (e.NewItems != null)
            {
                foreach (AssistantModel item in e.NewItems)
                {
                    item.PropertyChanged += AssistantItem_PropertyChanged;
                }
            }
        }

        private void AssistantItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Handle assistant item property changes if needed
        }

        private bool CanSaveToGoogleSheets()
        {
            // Can save if basic fields are filled
            return !string.IsNullOrWhiteSpace(Uid) && SelectedDate.HasValue;
        }

        private void SaveToGoogleSheetsAsync()
        {
            StatusMessage = "Saving to Google Sheets...";

            Task.Run(async () =>
            {
                try
                {
                    var googleSheetsService = new Services.GoogleSheetsService();

                    // Try to load config from application directory
                    string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "GoogleSheetsConfig.json");

                    bool initialized = await googleSheetsService.InitializeAsync(configPath);

                    if (!initialized)
                    {
                        StatusMessage = "✗ Google Sheets is not enabled in config. Please configure GoogleSheetsConfig.json";
                        return;
                    }

                    // Create purchase request model from form data
                    var purchaseRequest = new PurchaseRequestModel
                    {
                        UserId = Uid,
                        GeneralTreatmentDesc = TreatmentDescription,
                        TreatmentType = SelectedTreatmentType ?? TreatmentType.Other,
                        DoctorName = SelectedDoctor.Value,
                        CreatedAt = GetDateTimeFromInputs(),
                        TotalCost = TotalCost,
                        AltAssistantName = new List<string>(),
                        CostDetails = CostsList.ToList()
                    };

                    // Add assistant names to AltAssistantName list
                    foreach (var assistant in AssistantsList)
                    {
                        string assistantName = assistant.SelectedAssistant == AssistantName.Other 
                            ? (assistant.CustomAssistantName ?? "")
                            : AssistantNameHelper.GetDisplayName(assistant.SelectedAssistant);
                        purchaseRequest.AltAssistantName.Add(assistantName);
                    }

                    // Append to Google Sheet
                    bool success = await googleSheetsService.AppendPurchaseRequestAsync(purchaseRequest);

                    if (success)
                    {
                        StatusMessage = "✓ Successfully saved to Google Sheets!";
                    }
                    else
                    {
                        StatusMessage = "✗ Failed to save to Google Sheets";
                    }
                }
                catch (Exception ex)
                {
                    StatusMessage = $"✗ Error saving to Google Sheets: {ex.Message}";
                }
            });
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}
