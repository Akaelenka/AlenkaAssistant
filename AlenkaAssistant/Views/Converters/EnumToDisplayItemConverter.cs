using System;
using System.Globalization;
using System.Windows.Data;
using AlenkaAssistant.Models;

namespace AlenkaAssistant.Views.Converters
{
    /// <summary>
    /// Converts between enum values and DisplayItem wrapper objects for ComboBox binding.
    /// This allows the SelectedItem property to work with enum values while displaying friendly names.
    /// </summary>
    public class EnumToDisplayItemConverter<T> : IValueConverter where T : struct, System.Enum
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            // If it's already a DisplayItem, return it
            if (value is DisplayItem<T> displayItem)
                return displayItem;

            // If it's an enum value, wrap it in DisplayItem
            if (value is T enumValue)
            {
                return new DisplayItem<T>(enumValue, GetDisplayName(enumValue));
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DisplayItem<T> displayItem)
                return displayItem.Value;

            return null;
        }

        private string GetDisplayName(T enumValue)
        {
            if (typeof(T) == typeof(DoctorName))
            {
                return DoctorNameHelper.GetDisplayName((DoctorName)(object)enumValue);
            }
            else if (typeof(T) == typeof(AssistantName))
            {
                return AssistantNameHelper.GetDisplayName((AssistantName)(object)enumValue);
            }
            else if (typeof(T) == typeof(TreatmentType))
            {
                return TreatmentTypeHelper.GetDisplayName((TreatmentType)(object)enumValue);
            }

            return enumValue.ToString();
        }
    }

    /// <summary>
    /// Specialized converter for DoctorName enum
    /// </summary>
    public class DoctorNameToDisplayItemConverter : EnumToDisplayItemConverter<DoctorName>
    {
    }

    /// <summary>
    /// Specialized converter for AssistantName enum
    /// </summary>
    public class AssistantNameToDisplayItemConverter : EnumToDisplayItemConverter<AssistantName>
    {
    }

    /// <summary>
    /// Specialized converter for TreatmentType enum
    /// </summary>
    public class TreatmentTypeToDisplayItemConverter : EnumToDisplayItemConverter<TreatmentType>
    {
    }
}
