using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AlenkaAssistant.Models
{
    public enum AssistantName
    {
        None,
        Pavela,
        Ratih,
        Ana,
        Other
    }

    /// <summary>
    /// Helper class for AssistantName display mappings
    /// </summary>
    public static class AssistantNameHelper
    {
        /// <summary>
        /// Gets the display name for an AssistantName enum value
        /// </summary>
        public static string GetDisplayName(AssistantName assistantName)
        {
            return assistantName switch
            {
                AssistantName.None => "Tidak Ada",
                AssistantName.Pavela => "Pavela",
                AssistantName.Ratih => "Ratih",
                AssistantName.Ana => "Ana",
                AssistantName.Other => "Lainnya",
                _ => assistantName.ToString()
            };
        }

        /// <summary>
        /// Creates a collection of DisplayItem for ComboBox binding
        /// </summary>
        public static ObservableCollection<DisplayItem<AssistantName>> GetDisplayItems()
        {
            return new ObservableCollection<DisplayItem<AssistantName>>
            {
                new DisplayItem<AssistantName>(AssistantName.None, "Tidak Ada"),
                new DisplayItem<AssistantName>(AssistantName.Pavela, "Pavela"),
                new DisplayItem<AssistantName>(AssistantName.Ratih, "Ratih"),
                new DisplayItem<AssistantName>(AssistantName.Ana, "Ana"),
                new DisplayItem<AssistantName>(AssistantName.Other, "Lainnya")
            };
        }
    }
}
