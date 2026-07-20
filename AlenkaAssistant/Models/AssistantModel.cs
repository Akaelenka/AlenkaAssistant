using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Model for assistant selection with optional custom name
    /// </summary>
    public class AssistantModel : INotifyPropertyChanged
    {
        private AssistantName _selectedAssistant;
        private string? _customAssistantName;

        public AssistantName SelectedAssistant
        {
            get => _selectedAssistant;
            set
            {
                if (_selectedAssistant != value)
                {
                    _selectedAssistant = value;
                    OnPropertyChanged(nameof(SelectedAssistant));
                    OnPropertyChanged(nameof(ShowCustomInput));
                }
            }
        }

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
        /// Indicates whether custom input should be shown
        /// </summary>
        public bool ShowCustomInput => SelectedAssistant == AssistantName.Other;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
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
        /// Creates a collection of DisplayItem for ComboBox binding (excluding None)
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

        /// <summary>
        /// Creates a collection of DisplayItem for assistant list (excluding None)
        /// </summary>
        public static ObservableCollection<DisplayItem<AssistantName>> GetDisplayItemsForList()
        {
            return new ObservableCollection<DisplayItem<AssistantName>>
            {
                new DisplayItem<AssistantName>(AssistantName.Pavela, "Pavela"),
                new DisplayItem<AssistantName>(AssistantName.Ratih, "Ratih"),
                new DisplayItem<AssistantName>(AssistantName.Ana, "Ana"),
                new DisplayItem<AssistantName>(AssistantName.Other, "Lainnya")
            };
        }
    }
}
