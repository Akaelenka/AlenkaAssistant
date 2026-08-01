using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using AlenkaAssistant.Services;
using AlenkaAssistant.Views;

namespace AlenkaAssistant.ViewModels
{
    public class AddPatientDialogViewModel : INotifyPropertyChanged
    {
        private string _rmNumber;
        private string _patientName;
        private string _rmSuggestion;
        private bool _isLoadingLastRm;
        private string _errorMessage;
        private bool _hasError;
        private bool _canConfirm;

        private readonly AddPatientService _addPatientService;
        private readonly AddPatientDialog _dialog;

        public string RmNumber
        {
            get => _rmNumber;
            set
            {
                if (_rmNumber != value)
                {
                    _rmNumber = value;
                    OnPropertyChanged(nameof(RmNumber));
                    UpdateCanConfirm();
                }
            }
        }

        public string PatientName
        {
            get => _patientName;
            set
            {
                if (_patientName != value)
                {
                    _patientName = value;
                    OnPropertyChanged(nameof(PatientName));
                    UpdateCanConfirm();
                }
            }
        }

        public string RmSuggestion
        {
            get => _rmSuggestion;
            set
            {
                if (_rmSuggestion != value)
                {
                    _rmSuggestion = value;
                    OnPropertyChanged(nameof(RmSuggestion));
                }
            }
        }

        public bool IsLoadingLastRm
        {
            get => _isLoadingLastRm;
            set
            {
                if (_isLoadingLastRm != value)
                {
                    _isLoadingLastRm = value;
                    OnPropertyChanged(nameof(IsLoadingLastRm));
                }
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged(nameof(ErrorMessage));
                }
            }
        }

        public bool HasError
        {
            get => _hasError;
            set
            {
                if (_hasError != value)
                {
                    _hasError = value;
                    OnPropertyChanged(nameof(HasError));
                }
            }
        }

        public bool CanConfirm
        {
            get => _canConfirm;
            set
            {
                if (_canConfirm != value)
                {
                    _canConfirm = value;
                    OnPropertyChanged(nameof(CanConfirm));
                }
            }
        }

        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        public AddPatientDialogViewModel(AddPatientDialog dialog, AddPatientService addPatientService)
        {
            _dialog = dialog;
            _addPatientService = addPatientService;

            ConfirmCommand = new RelayCommand(_ => Confirm());
            CancelCommand = new RelayCommand(_ => Cancel());

            // Load the last RM number
            LoadLastRmAsync();
        }

        private async void LoadLastRmAsync()
        {
            IsLoadingLastRm = true;
            HasError = false;
            ErrorMessage = "";

            try
            {
                var response = await _addPatientService.GetLastRmAsync();

                if (response.Success && !string.IsNullOrWhiteSpace(response.NextRm))
                {
                    RmNumber = response.NextRm;
                    RmSuggestion = $"💡 Saran: {response.NextRm} (nomor RM berikutnya setelah {response.LastRm})";
                }
                else
                {
                    // Default suggestion if service fails
                    RmNumber = "A.0001";
                    RmSuggestion = "💡 Masukkan nomor RM baru. Format: A.0001, A.0002, dst.";

                    if (!string.IsNullOrWhiteSpace(response.Error))
                    {
                        HasError = true;
                        ErrorMessage = $"⚠ Tidak bisa mendapatkan nomor RM terakhir: {response.Error}. Silakan masukkan secara manual.";
                    }
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"⚠ Error: {ex.Message}. Silakan masukkan nomor RM secara manual.";
                RmNumber = "A.0001";
                RmSuggestion = "💡 Masukkan nomor RM baru. Format: A.0001, A.0002, dst.";
            }
            finally
            {
                IsLoadingLastRm = false;
                UpdateCanConfirm();
            }
        }

        private void UpdateCanConfirm()
        {
            CanConfirm = !IsLoadingLastRm 
                && !string.IsNullOrWhiteSpace(RmNumber) 
                && !string.IsNullOrWhiteSpace(PatientName);
        }

        private void Confirm()
        {
            if (!CanConfirm)
                return;

            _dialog.RmNumber = RmNumber;
            _dialog.PatientName = PatientName;
            _dialog.Confirmed = true;
            _dialog.DialogResult = true;
            _dialog.Close();
        }

        private void Cancel()
        {
            _dialog.Confirmed = false;
            _dialog.DialogResult = false;
            _dialog.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
