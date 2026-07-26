using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AlenkaAssistant.Views.Converters
{
    /// <summary>
    /// Converts string to Visibility
    /// - Non-empty string → Visible
    /// - Null or empty string → Collapsed
    /// </summary>
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && !string.IsNullOrWhiteSpace(str))
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
