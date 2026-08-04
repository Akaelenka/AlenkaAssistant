using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AlenkaAssistant.Views.Converters
{
    public class StatusMessageColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string message && !string.IsNullOrWhiteSpace(message))
            {
                // Red for errors (messages starting with ✗)
                if (message.StartsWith("✗"))
                {
                    return new SolidColorBrush(Color.FromRgb(244, 67, 54)); // #F44336 - Red
                }
                // Green for success (messages starting with ✓)
                else if (message.StartsWith("✓"))
                {
                    return new SolidColorBrush(Color.FromRgb(76, 175, 80)); // #4CAF50 - Green
                }
            }
            // Default to gray
            return new SolidColorBrush(Color.FromRgb(85, 85, 85)); // #555555 - Dark Gray
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
