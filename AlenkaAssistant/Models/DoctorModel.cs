using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AlenkaAssistant.Models
{
    public enum DoctorName
    {
        DrgNovi,
        DrgFarasinta,
        DrgDiozola,
        Other
    }

    /// <summary>
    /// Helper class for DoctorName display mappings
    /// </summary>
    public static class DoctorNameHelper
    {
        /// <summary>
        /// Gets the display name for a DoctorName enum value
        /// </summary>
        public static string GetDisplayName(DoctorName doctorName)
        {
            return doctorName switch
            {
                DoctorName.DrgNovi => "drg. Novi",
                DoctorName.DrgFarasinta => "drg. Farasinta",
                DoctorName.DrgDiozola => "drg. Diozola",
                DoctorName.Other => "Lainnya",
                _ => doctorName.ToString()
            };
        }

        /// <summary>
        /// Creates a collection of DisplayItem for ComboBox binding
        /// </summary>
        public static ObservableCollection<DisplayItem<DoctorName>> GetDisplayItems()
        {
            return new ObservableCollection<DisplayItem<DoctorName>>
            {
                new DisplayItem<DoctorName>(DoctorName.DrgNovi, "Drg. Novi"),
                new DisplayItem<DoctorName>(DoctorName.DrgFarasinta, "Drg. Farasinta"),
                new DisplayItem<DoctorName>(DoctorName.DrgDiozola, "Drg. Diozola"),
                new DisplayItem<DoctorName>(DoctorName.Other, "Lainnya")
            };
        }
    }
}
