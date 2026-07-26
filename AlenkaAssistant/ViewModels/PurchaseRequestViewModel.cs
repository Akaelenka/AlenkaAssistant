using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Windows;
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
        private TreatmentType? _selectedTreatmentType;
        private DateTime? _selectedDate;
        private string _selectedTime;
        private string _statusMessage;
        private string _customDoctorName;
        private bool _showCustomDoctor;

        private PatientLookupService _patientLookupService;
        private string _patientName;
        private string _patientLookupMessage;
        private bool _isPatientLookupLoading;

        public string PatientName
        {
            get => _patientName;
            set
            {
                if (_patientName != value)
                {
                    _patientName = value;
                    OnPropertyChanged(nameof(PatientName));
                }
            }
        }

        public string PatientLookupMessage
        {
            get => _patientLookupMessage;
            set
            {
                if (_patientLookupMessage != value)
                {
                    _patientLookupMessage = value;
                    OnPropertyChanged(nameof(PatientLookupMessage));
                }
            }
        }

        public bool IsPatientLookupLoading
        {
            get => _isPatientLookupLoading;
            set
            {
                if (_isPatientLookupLoading != value)
                {
                    _isPatientLookupLoading = value;
                    OnPropertyChanged(nameof(IsPatientLookupLoading));
                }
            }
        }

        public string Uid
        {
            get => _uid;
            set
            {
                if (_uid != value)
                {
                    _uid = value;
                    OnPropertyChanged(nameof(Uid));

                    // Trigger patient lookup when RM changes
                    if (!string.IsNullOrWhiteSpace(value) && _patientLookupService != null)
                    {
                        LookupPatientAsync(value).ConfigureAwait(false);
                    }
                    else
                    {
                        // Clear patient info if UID is empty
                        PatientName = "";
                        PatientLookupMessage = "";
                    }
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
        public ObservableCollection<string> AssistantNamesForList { get; private set; }
        public ObservableCollection<string> DoctorNames { get; private set; }
        public ObservableCollection<CostModel> CostsList { get; }
        public ObservableCollection<AssistantModel> AssistantsList { get; }

        private string _selectedAssistantName;
        public string SelectedAssistantName
        {
            get => _selectedAssistantName;
            set
            {
                if (_selectedAssistantName != value)
                {
                    _selectedAssistantName = value;
                    OnPropertyChanged(nameof(SelectedAssistantName));
                }
            }
        }

        private string _selectedDoctorName;
        public string SelectedDoctorName
        {
            get => _selectedDoctorName;
            set
            {
                if (_selectedDoctorName != value)
                {
                    _selectedDoctorName = value;
                    OnPropertyChanged(nameof(SelectedDoctorName));

                    // Update visibility of custom doctor textbox
                    ShowCustomDoctor = value == "Other";
                    if (value != "Other")
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
                    total += item.Cost - item.Discount;
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
        public ICommand SaveLocalCommand { get; }
        public ICommand PrintCommand { get; }

        public PurchaseRequestViewModel()
        {
            // Initialize treatment types (still enum-based)
            TreatmentTypes = TreatmentTypeHelper.GetDisplayItems();

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

            // Load dropdown configuration and initialize patient lookup service
            LoadDropdownConfigAsync();

            // Set defaults
            SelectedDate = DateTime.Today;
            SelectedTime = DateTime.Now.ToString("HH:mm");

            // Initialize commands
            SubmitCommand = new RelayCommand(_ => SubmitRequest(), _ => CanSubmit());
            CancelCommand = new RelayCommand(_ => CancelRequestAsync());
            AddCostCommand = new RelayCommand(_ => AddCost());
            RemoveCostCommand = new RelayCommand(obj => RemoveCost(obj), obj => CanRemoveCost(obj));
            AddAssistantCommand = new RelayCommand(_ => AddAssistant());
            RemoveAssistantCommand = new RelayCommand(obj => RemoveAssistant(obj));
            SaveToGoogleSheetsCommand = new RelayCommand(_ => SaveToGoogleSheetsAsync(), _ => CanSaveToGoogleSheets());
            SaveLocalCommand = new RelayCommand(_ => SaveLocalAsync(), _ => CanSaveLocal());
            PrintCommand = new RelayCommand(_ => Print(), _ => CanPrint());
        }

        private async void LoadDropdownConfigAsync()
        {
            try
            {
                var dropdownConfigService = new DropdownConfigService();
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "GoogleSheetsConfig.json");

                bool loaded = await dropdownConfigService.LoadDropdownConfigAsync(configPath);

                if (loaded)
                {
                    AssistantNamesForList = dropdownConfigService.GetAssistantNames();
                    DoctorNames = dropdownConfigService.GetDoctorNames();
                    OnPropertyChanged(nameof(AssistantNamesForList));
                    OnPropertyChanged(nameof(DoctorNames));

                    // Set default doctor name to first in config
                    if (DoctorNames?.Count > 0)
                    {
                        SelectedDoctorName = DoctorNames[0];
                    }

                    // Initialize PatientLookupService with deployment URL
                    try
                    {
                        string deploymentUrl = dropdownConfigService.GetDeploymentUrl();
                        if (!string.IsNullOrWhiteSpace(deploymentUrl))
                        {
                            _patientLookupService = new PatientLookupService(deploymentUrl);
                            System.Diagnostics.Debug.WriteLine("[ViewModel] PatientLookupService initialized");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[ViewModel] Error initializing PatientLookupService: {ex.Message}");
                    }
                }
                else
                {
                    // Fallback to defaults if config loading fails
                    AssistantNamesForList = new ObservableCollection<string> { "Pavela", "Ana", "Other" };
                    DoctorNames = new ObservableCollection<string> { "Drg. Novi", "Drg. Rina", "Other" };
                    SelectedDoctorName = DoctorNames[0];
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading dropdown config: {ex.Message}");
                // Use defaults
                AssistantNamesForList = new ObservableCollection<string> { "Pavela", "Ana", "Other" };
                DoctorNames = new ObservableCollection<string> { "Drg. Novi", "Drg. Rina", "Other" };
                SelectedDoctorName = DoctorNames[0];
            }
        }

        /// <summary>
        /// Lookup patient name by RM number from the patient sheet
        /// </summary>
        private async Task LookupPatientAsync(string rmNumber)
        {
            try
            {
                // Clear previous results
                PatientName = "";
                PatientLookupMessage = "";
                IsPatientLookupLoading = true;

                if (_patientLookupService == null)
                {
                    System.Diagnostics.Debug.WriteLine("[ViewModel] PatientLookupService not initialized");
                    IsPatientLookupLoading = false;
                    return;
                }

                if (string.IsNullOrWhiteSpace(rmNumber))
                {
                    IsPatientLookupLoading = false;
                    return;
                }

                // Extract just the number part for searching
                string searchValue = NoRmFormatter.GetSearchValue(rmNumber);
                System.Diagnostics.Debug.WriteLine($"[ViewModel] Looking up patient with RM: {rmNumber} (search value: {searchValue})");

                var result = await _patientLookupService.LookupPatientAsync(
                    sheetName: "NoRM",
                    rmNumber: searchValue,
                    searchColumn: 0,  // Column A
                    resultColumn: 1   // Column B
                );

                Application.Current.Dispatcher.Invoke(() =>
                {
                    IsPatientLookupLoading = false;

                    if (result.Success && result.Found)
                    {
                        PatientName = result.PatientName;
                        PatientLookupMessage = ""; // Clear message on success
                        System.Diagnostics.Debug.WriteLine($"[ViewModel] Patient found: {result.PatientName}");
                    }
                    else if (result.Success && !result.Found)
                    {
                        PatientName = "";
                        PatientLookupMessage = "⚠️ Patient not found"; // Warning in orange/yellow
                        System.Diagnostics.Debug.WriteLine("[ViewModel] Patient not found");
                    }
                    else
                    {
                        PatientName = "";
                        PatientLookupMessage = $"❌ Error: {result.Error}"; // Error in red
                        System.Diagnostics.Debug.WriteLine($"[ViewModel] Lookup error: {result.Error}");
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ViewModel] Exception in LookupPatientAsync: {ex.Message}");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    IsPatientLookupLoading = false;
                    PatientName = "";
                    PatientLookupMessage = $"❌ Error: {ex.Message}";
                });
            }
        }

        private bool CanSubmit()
        {
            // Check basic fields
            if (!(!string.IsNullOrWhiteSpace(Uid) 
                && SelectedDate.HasValue 
                && !string.IsNullOrWhiteSpace(SelectedDoctorName)))
            {
                return false;
            }

            // Check if custom doctor name is required and provided
            if (SelectedDoctorName == "Other" && string.IsNullOrWhiteSpace(CustomDoctorName))
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
                if (SelectedDoctorName == "Other" && string.IsNullOrWhiteSpace(CustomDoctorName))
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
                    UserId = NoRmFormatter.FormatRmForOutput(Uid), // Format RM as A.xxxx
                    GeneralTreatmentDesc = "",
                    AssistantName = AssistantName.None,
                    DoctorName = ConvertDoctorNameStringToEnum(SelectedDoctorName),
                    CreatedAt = GetDateTimeFromInputs(),
                    AltAssistantName = new List<string>(),
                    CostDetails = CostsList.ToList()
                };

                // Add assistant names to AltAssistantName list
                foreach (var assistant in AssistantsList)
                {
                    string assistantName = assistant.SelectedAssistantName == "Other" 
                        ? (assistant.CustomAssistantName ?? "")
                        : assistant.SelectedAssistantName;
                    purchaseRequest.AltAssistantName.Add(assistantName);
                }

                // TODO: Save to database or API
                StatusMessage = $"✓ Purchase request submitted successfully for RM: {purchaseRequest.UserId}";
                // Clear form after successful submission
                ClearForm();
            }
            catch (Exception ex)
            {
                StatusMessage = $"✗ Error: {ex.Message}";
            }
        }

        private async void CancelRequestAsync()
        {
            // Delete from local storage
            var localDataService = new LocalDataService();
            var lastSavedRequest = await localDataService.LoadLastPurchaseRequestAsync();

            if (lastSavedRequest != null)
            {
                await localDataService.DeletePurchaseRequestAsync(lastSavedRequest.Id);
            }

            ClearForm();
            StatusMessage = "✓ Data cleared. Batal successful.";
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
                return DateTime.Today;
            }
        }

        /// <summary>
        /// Get Indonesian month name from month number
        /// </summary>
        private string GetIndonesianMonth(int month)
        {
            return month switch
            {
                1 => "JANUARI",
                2 => "FEBRUARI",
                3 => "MARET",
                4 => "APRIL",
                5 => "MEI",
                6 => "JUNI",
                7 => "JULI",
                8 => "AGUSTUS",
                9 => "SEPTEMBER",
                10 => "OKTOBER",
                11 => "NOVEMBER",
                12 => "DESEMBER",
                _ => ""
            };
        }

        private DoctorName ConvertDoctorNameStringToEnum(string doctorNameString)
        {
            if (string.IsNullOrWhiteSpace(doctorNameString))
                return DoctorName.Other;

            // Try to find a matching doctor by display name
            foreach (DoctorName enumVal in System.Enum.GetValues(typeof(DoctorName)))
            {
                if (DoctorNameHelper.GetDisplayName(enumVal) == doctorNameString || 
                    DoctorNameHelper.GetDisplayName(enumVal).Equals(doctorNameString, System.StringComparison.OrdinalIgnoreCase))
                {
                    return enumVal;
                }
            }

            return DoctorName.Other;
        }

        private void ClearForm()
        {
            Uid = string.Empty;
            SelectedDoctorName = DoctorNames?.Count > 0 ? DoctorNames[0] : "Other";
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
            var newCost = new CostModel 
            { 
                TreatmentDesc = string.Empty, 
                Cost = 0,
                TreatmentType = null,
                RM = Uid,
                Month = GetIndonesianMonth(SelectedDate?.Month ?? DateTime.Now.Month)
            };
            CostsList.Add(newCost);
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
            // Update total cost whenever a cost item's Cost, Discount, or TreatmentDesc property changes
            if (e.PropertyName == nameof(CostModel.Cost) || e.PropertyName == nameof(CostModel.TreatmentDesc) || e.PropertyName == nameof(CostModel.Discount))
            {
                OnPropertyChanged(nameof(TotalCost));
            }
        }

        private void AddAssistant()
        {
            var defaultAssistant = AssistantNamesForList?.Count > 0 ? AssistantNamesForList[0] : "Other";
            AssistantsList.Add(new AssistantModel { SelectedAssistantName = defaultAssistant, CustomAssistantName = string.Empty });
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
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            StatusMessage = "✗ Google Sheets is not enabled in config. Please configure GoogleSheetsConfig.json";
                        });
                        return;
                    }

                    // Create purchase request model from form data
                    var purchaseRequest = new PurchaseRequestModel
                    {
                        UserId = NoRmFormatter.FormatRmForOutput(Uid), // Format RM as A.xxxx
                        GeneralTreatmentDesc = "",
                        TreatmentType = TreatmentType.Lainnya,
                        DoctorName = ConvertDoctorNameStringToEnum(SelectedDoctorName),
                        CreatedAt = GetDateTimeFromInputs(),
                        TotalCost = TotalCost,
                        AltAssistantName = new List<string>(),
                        CostDetails = CostsList.ToList(),
                        AltDoctorName = SelectedDoctorName == "Other" ? CustomDoctorName : null
                    };

                    // Add assistant names to AltAssistantName list
                    foreach (var assistant in AssistantsList)
                    {
                        string assistantName = assistant.SelectedAssistantName == "Other" 
                            ? (assistant.CustomAssistantName ?? "")
                            : assistant.SelectedAssistantName;
                        purchaseRequest.AltAssistantName.Add(assistantName);
                    }

                    // Append to Google Sheet
                    bool success = await googleSheetsService.AppendPurchaseRequestAsync(purchaseRequest);

                    // Use Dispatcher to update UI from background thread
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (success)
                        {
                            StatusMessage = "✓ Successfully saved to Google Sheets!";

                            // Delete from local storage after successful Google submission
                            Task.Run(async () =>
                            {
                                try
                                {
                                    var localDataService = new LocalDataService();
                                    var lastSavedRequest = await localDataService.LoadLastPurchaseRequestAsync();

                                    if (lastSavedRequest != null)
                                    {
                                        await localDataService.DeletePurchaseRequestAsync(lastSavedRequest.Id);
                                    }
                                }
                                catch { }
                            });

                            // Clear form after successful submission
                            ClearForm();
                        }
                        else
                        {
                            StatusMessage = "✗ Failed to save to Google Sheets";
                        }
                    });
                }
                catch (Exception ex)
                {
                    // Use Dispatcher to update UI from background thread
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        StatusMessage = $"✗ Error saving to Google Sheets: {ex.Message}";
                    });
                }
            });
        }

        private bool CanSaveLocal()
        {
            // Can save if basic fields are filled
            return !string.IsNullOrWhiteSpace(Uid) && SelectedDate.HasValue;
        }

        private void SaveLocalAsync()
        {
            StatusMessage = "Saving locally...";

            Task.Run(async () =>
            {
                try
                {
                    var localDataService = new LocalDataService();

                    // Create purchase request model from form data
                    var purchaseRequest = new PurchaseRequestModel
                    {
                        UserId = NoRmFormatter.FormatRmForOutput(Uid), // Format RM as A.xxxx
                        GeneralTreatmentDesc = "",
                        TreatmentType = TreatmentType.Lainnya,
                        DoctorName = ConvertDoctorNameStringToEnum(SelectedDoctorName),
                        CreatedAt = GetDateTimeFromInputs(),
                        TotalCost = TotalCost,
                        AltAssistantName = new List<string>(),
                        CostDetails = new List<CostModel>(),
                        AltDoctorName = SelectedDoctorName == "Other" ? CustomDoctorName : null
                    };

                    // Add assistant names to AltAssistantName list
                    foreach (var assistant in AssistantsList)
                    {
                        string assistantName = assistant.SelectedAssistantName == "Other" 
                            ? (assistant.CustomAssistantName ?? "")
                            : assistant.SelectedAssistantName;
                        purchaseRequest.AltAssistantName.Add(assistantName);
                    }

                    // Add costs to CostDetails
                    foreach (var cost in CostsList)
                    {
                        purchaseRequest.CostDetails.Add(new CostModel 
                        { 
                            TreatmentDesc = cost.TreatmentDesc, 
                            Cost = cost.Cost,
                            TreatmentType = cost.TreatmentType,
                            RM = cost.RM,
                            Month = cost.Month
                        });
                    }

                    // Save to local storage
                    bool success = await localDataService.SavePurchaseRequestAsync(purchaseRequest);

                    // Use Dispatcher to update UI from background thread
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (success)
                        {
                            StatusMessage = "✓ Successfully saved locally!";
                            // Do NOT clear form after local save - user wants to keep editing
                        }
                        else
                        {
                            StatusMessage = "✗ Failed to save locally";
                        }
                    });
                }
                catch (Exception ex)
                {
                    // Use Dispatcher to update UI from background thread
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        StatusMessage = $"✗ Error saving locally: {ex.Message}";
                    });
                }
            });
        }

        private bool CanPrint()
        {
            // Can print if basic fields are filled
            return !string.IsNullOrWhiteSpace(Uid) && CostsList.Count > 0;
        }

        private void Print()
        {
            try
            {
                StatusMessage = "Generating print preview...";

                var printService = new PrintService();

                // Generate the invoice document from CURRENT form data (without modifying it)
                var doc = printService.GenerateInvoiceDocument(BuildPurchaseRequestModel(), PatientName);

                // Create and show the print preview window
                // NOTE: This does NOT clear or modify any form data - it's preview only
                var previewWindow = new global::AlenkaAssistant.Views.PrintPreviewWindow(doc, "Alenka Invoice - Print Preview");
                previewWindow.ShowDialog();

                // After preview closes, form data remains unchanged
                StatusMessage = "Print preview closed";
            }
            catch (Exception ex)
            {
                StatusMessage = $"✗ Error printing: {ex.Message}";
            }
        }

        private PurchaseRequestModel BuildPurchaseRequestModel()
        {
            var model = new PurchaseRequestModel
            {
                UserId = NoRmFormatter.FormatRmForOutput(Uid),
                GeneralTreatmentDesc = "",
                TreatmentType = TreatmentType.Lainnya,
                DoctorName = ConvertDoctorNameStringToEnum(SelectedDoctorName),
                CreatedAt = GetDateTimeFromInputs(),
                TotalCost = TotalCost,
                AltAssistantName = new List<string>(),
                CostDetails = new List<CostModel>(),
                AltDoctorName = SelectedDoctorName == "Other" ? CustomDoctorName : null
            };

            // Add assistant names
            foreach (var assistant in AssistantsList)
            {
                string assistantName = assistant.SelectedAssistantName == "Other" 
                    ? (assistant.CustomAssistantName ?? "")
                    : assistant.SelectedAssistantName;
                model.AltAssistantName.Add(assistantName);
            }

            // Add costs
            foreach (var cost in CostsList)
            {
                model.CostDetails.Add(new CostModel 
                { 
                    TreatmentDesc = cost.TreatmentDesc, 
                    Cost = cost.Cost,
                    TreatmentType = cost.TreatmentType,
                    Discount = cost.Discount,
                    RM = cost.RM,
                    Month = cost.Month
                });
            }

            return model;
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

    }
}
