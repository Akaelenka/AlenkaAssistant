using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AlenkaAssistant.Models
{
    public class CostModel : INotifyPropertyChanged
    {
        private string? _treatmentDesc;
        private int _cost;
        private TreatmentType? _treatmentType;
        private string? _rm;
        private string? _month;
        private int _itemCount;
        private int _discount;

        public string? TreatmentDesc
        {
            get => _treatmentDesc;
            set
            {
                if (_treatmentDesc != value)
                {
                    _treatmentDesc = value;
                    OnPropertyChanged(nameof(TreatmentDesc));
                }
            }
        }

        public int Cost
        {
            get => _cost;
            set
            {
                if (_cost != value)
                {
                    _cost = value;
                    OnPropertyChanged(nameof(Cost));
                    OnPropertyChanged(nameof(TotalAmount));
                }
            }
        }

        /// <summary>
        /// Treatment type for this detail row
        /// </summary>
        public TreatmentType? TreatmentType
        {
            get => _treatmentType;
            set
            {
                if (_treatmentType != value)
                {
                    _treatmentType = value;
                    OnPropertyChanged(nameof(TreatmentType));
                }
            }
        }

        /// <summary>
        /// RM number (copied from main patient record)
        /// </summary>
        public string? RM
        {
            get => _rm;
            set
            {
                if (_rm != value)
                {
                    _rm = value;
                    OnPropertyChanged(nameof(RM));
                }
            }
        }

        /// <summary>
        /// Month (extracted from the treatment date)
        /// </summary>
        public string? Month
        {
            get => _month;
            set
            {
                if (_month != value)
                {
                    _month = value;
                    OnPropertyChanged(nameof(Month));
                }
            }
        }

        /// <summary>
        /// Item count for this detail row
        /// </summary>
        public int ItemCount
        {
            get => _itemCount;
            set
            {
                if (_itemCount != value)
                {
                    _itemCount = value;
                    OnPropertyChanged(nameof(ItemCount));
                    OnPropertyChanged(nameof(TotalAmount));
                }
            }
        }

        /// <summary>
        /// Discount amount for this detail row
        /// </summary>
        public int Discount
        {
            get => _discount;
            set
            {
                if (_discount != value)
                {
                    _discount = value;
                    OnPropertyChanged(nameof(Discount));
                    OnPropertyChanged(nameof(TotalAmount));
                }
            }
        }

        /// <summary>
        /// Calculated total: (Cost × ItemCount) - Discount
        /// </summary>
        public int TotalAmount
        {
            get => (Cost * ItemCount) - Discount;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class AssistantItem : INotifyPropertyChanged
    {
        private string? _selectedAssistantName;
        private string? _customAssistantName;
        private bool _showCustomInput;

        /// <summary>
        /// Selected assistant name from the dropdown
        /// </summary>
        public string? SelectedAssistantName
        {
            get => _selectedAssistantName;
            set
            {
                if (_selectedAssistantName != value)
                {
                    _selectedAssistantName = value;
                    OnPropertyChanged(nameof(SelectedAssistantName));

                    // Show custom input if "Other" is selected
                    ShowCustomInput = value == "Other";
                    if (value != "Other")
                    {
                        CustomAssistantName = string.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// Custom name if "Other" is selected
        /// </summary>
        public string? CustomAssistantName
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

        /// <summary>
        /// Whether to show the custom input textbox
        /// </summary>
        public bool ShowCustomInput
        {
            get => _showCustomInput;
            set
            {
                if (_showCustomInput != value)
                {
                    _showCustomInput = value;
                    OnPropertyChanged(nameof(ShowCustomInput));
                }
            }
        }

        /// <summary>
        /// Get the final assistant name (either selected or custom)
        /// </summary>
        public string? GetFinalName()
        {
            if (SelectedAssistantName == "Other" && !string.IsNullOrWhiteSpace(CustomAssistantName))
            {
                return CustomAssistantName;
            }
            return SelectedAssistantName;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class PurchaseRequestModel
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? GeneralTreatmentDesc { get; set; }
        public TreatmentType TreatmentType { get; set; }
        public int TotalCost { get; set; }
        public List<string>? AssistantNames { get; set; }
        public string? DoctorName { get; set; }
        public List<CostModel>? CostDetails { get; set; }
        public bool IsSavedToPatients { get; set; } = false;
    }
}
