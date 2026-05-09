using System;
using System.Collections.Generic;
using System.Text;

namespace AlenkaAssistant.Scripts.Models
{
    public class CostModel
    {
        public int TreatmentDesc { get; set; }
        public int Cost { get; set; }
    }

    public class PurchaseRequestModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? GeneralTreatmentDesc { get; set; }
        public TreatmentType TreatmentType { get; set; }
        public int TotalCost { get; set; }
        public AssistantName[] AssistantName { get; set; }
        public List<string> AltAssistantName { get; set; }
        public DoctorName DoctorName { get; set; }
        public string? AltDoctorName { get; set; }
        public List<CostModel> CostDetails { get; set; }
    }
}
