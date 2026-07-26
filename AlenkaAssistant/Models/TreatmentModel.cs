using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AlenkaAssistant.Models
{
    public enum TreatmentType
    {
        Lainnya,
        Ortho,
        Tumpat,
        Exo,
        PSA_Mumi_Pasak,
        Medikasi,
        Pemeriksaan,
        Splinting,
        GT,
        Scaling,
        MJ,
        Capping,
        Bleaching
    }

    /// <summary>
    /// Helper class for TreatmentType display mappings
    /// </summary>
    public static class TreatmentTypeHelper
    {
        /// <summary>
        /// Gets the display name for a TreatmentType enum value, replacing underscores with slashes
        /// </summary>
        public static string GetDisplayName(TreatmentType treatmentType)
        {
            return treatmentType.ToString().Replace("_", " / ");
        }

        /// <summary>
        /// Creates a collection of DisplayItem for ComboBox binding
        /// </summary>
        public static ObservableCollection<DisplayItem<TreatmentType>> GetDisplayItems()
        {
            var items = new ObservableCollection<DisplayItem<TreatmentType>>();

            foreach (TreatmentType type in System.Enum.GetValues(typeof(TreatmentType)))
            {
                items.Add(new DisplayItem<TreatmentType>(type, GetDisplayName(type)));
            }

            return items;
        }
    }
}
