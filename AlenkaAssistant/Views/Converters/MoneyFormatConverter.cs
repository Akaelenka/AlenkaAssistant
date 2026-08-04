using System;
using System.Globalization;
using System.Windows.Data;
using System.Text.RegularExpressions;

namespace AlenkaAssistant.Views.Converters
{
    /// <summary>
    /// Converter for formatting money input/output with Indonesian format (dots for thousands separator)
    /// Example: 1000 -> "1.000", 1000000 -> "1.000.000"
    /// </summary>
    public class MoneyFormatConverter : IValueConverter
    {
        /// <summary>
        /// Convert from int to formatted string (Model -> UI)
        /// Converts: 1000 -> "1.000"
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";

            if (int.TryParse(value.ToString(), out int numValue))
            {
                // Format with Indonesian thousands separator (dot)
                return numValue.ToString("N0", CultureInfo.GetCultureInfo("id-ID"));
            }

            return value.ToString();
        }

        /// <summary>
        /// Convert from formatted string back to int (UI -> Model)
        /// Converts: "1.000" -> 1000, "1.000.000" -> 1000000
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return 0;

            string stringValue = value.ToString().Trim();

            // Remove all dots (thousands separator in Indonesian format)
            stringValue = stringValue.Replace(".", "");

            // Try to parse as integer
            if (int.TryParse(stringValue, out int result))
            {
                return result;
            }

            return 0;
        }
    }
}
