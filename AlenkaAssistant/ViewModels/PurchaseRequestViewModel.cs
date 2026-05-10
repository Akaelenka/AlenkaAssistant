using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using AlenkaAssistant.Models;

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
        private AssistantName? _selectedAssistant;
        private DoctorName? _selectedDoctor;
        private DateTime? _selectedDate;
        private string _selectedTime;
        private string _statusMessage;
        private string _customAssistantName;
        private string _customDoctorName;
        private bool _showCustomAssistant;
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
        public ObservableCollection<DisplayItem<AssistantName>> AssistantNames { get; }
        public ObservableCollection<DisplayItem<DoctorName>> DoctorNames { get; }

        public AssistantName? SelectedAssistant
        {
            get => _selectedAssistant;
            set
            {
                if (_selectedAssistant != value)
                {
                    _selectedAssistant = value;
                    OnPropertyChanged(nameof(SelectedAssistant));

                    // Update visibility of custom assistant textbox
                    ShowCustomAssistant = value == AssistantName.Other;
                    if (value != AssistantName.Other)
                    {
                        CustomAssistantName = string.Empty;
                    }
                }
            }
        }

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

        public string CustomAssistantName
        {
            get => _customAssistantName;
            set
            {
                if (_customAssistantName != value)
                {
                    _customAssistantName = value;
                    OnPropertyChanged(nameof(CustomAssistantName));
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

        public bool ShowCustomAssistant
        {
            get => _showCustomAssistant;
            set
            {
                if (_showCustomAssistant != value)
                {
                    _showCustomAssistant = value;
                    OnPropertyChanged(nameof(ShowCustomAssistant));
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

        public ICommand SubmitCommand { get; }
        public ICommand CancelCommand { get; }

        public PurchaseRequestViewModel()
        {
            // Initialize collections using helper methods
            TreatmentTypes = TreatmentTypeHelper.GetDisplayItems();
            AssistantNames = AssistantNameHelper.GetDisplayItems();
            DoctorNames = DoctorNameHelper.GetDisplayItems();

            // Set defaults
            SelectedDoctor = DoctorName.DrgNovi;
            SelectedDate = DateTime.Today;
            SelectedTime = DateTime.Now.ToString("HH:mm");

            // Initialize commands
            SubmitCommand = new RelayCommand(_ => SubmitRequest(), _ => CanSubmit());
            CancelCommand = new RelayCommand(_ => CancelRequest());
        }

        private bool CanSubmit()
        {
            // Check basic fields
            if (!(!string.IsNullOrWhiteSpace(Uid) 
                && SelectedAssistant.HasValue 
                && SelectedDate.HasValue 
                && SelectedDoctor.HasValue))
            {
                return false;
            }

            // Check if custom assistant name is required and provided
            if (SelectedAssistant == AssistantName.Other && string.IsNullOrWhiteSpace(CustomAssistantName))
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
                if (SelectedAssistant == AssistantName.Other && string.IsNullOrWhiteSpace(CustomAssistantName))
                {
                    StatusMessage = "✗ Please enter a custom assistant name.";
                }
                else if (SelectedDoctor == DoctorName.Other && string.IsNullOrWhiteSpace(CustomDoctorName))
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
                    AssistantName = SelectedAssistant.Value,
                    DoctorName = SelectedDoctor.Value,
                    CreatedAt = GetDateTimeFromInputs()
                };

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
            SelectedAssistant = null;
            SelectedDoctor = null;
            SelectedDate = DateTime.Today;
            SelectedTime = DateTime.Now.ToString("HH:mm");
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}
