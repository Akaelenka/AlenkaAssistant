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
                }
            }
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
        public AssistantName? AssistantName { get; set; }
        public List<string>? AltAssistantName { get; set; }
        public DoctorName? DoctorName { get; set; }
        public string? AltDoctorName { get; set; }
        public List<CostModel>? CostDetails { get; set; }
    }
}
