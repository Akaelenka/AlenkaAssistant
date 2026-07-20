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
