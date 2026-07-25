using System;
using System.Globalization;
using System.Windows.Data;

namespace AlenkaAssistant.Views.Converters
{
    /// <summary>
    /// A pass-through converter for string values.
    /// Used for dropdowns that are populated from string lists (e.g., assistant names from config).
    /// </summary>
    public class StringPassThroughConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Just pass the string through as-is
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Just pass the string back as-is
            return value;
        }
    }
}
