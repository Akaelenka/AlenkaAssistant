using System;
using System.Text.RegularExpressions;

namespace AlenkaAssistant.Services
{
    /// <summary>
    /// Helper class for handling No RM formatting
    /// Supports formats: "A.xxxx" or "xxxx" or just numbers
    /// - Input: accepts various formats, auto-pads to 4 digits
    /// - Search: uses extracted padded number only (0032)
    /// - Output/Display: uses formatted "A.0032"
    /// </summary>
    public class NoRmFormatter
    {
        private const string RmPrefix = "A";
        private const int PaddedLength = 4; // Pad to 4 digits: 32 -> 0032

        /// <summary>
        /// Normalize RM input to just the padded number part
        /// Input: "A.32" or "32" or "A.0032" or "0032"
        /// Output: "0032" (padded to 4 digits)
        /// </summary>
        public static string ExtractRmNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            input = input.Trim();

            // Remove the "A." prefix if present
            if (input.StartsWith("A."))
                input = input.Substring(2);
            // Remove just "A" if present
            else if (input.StartsWith("A"))
                input = input.Substring(1);

            // Extract only digits
            string numbersOnly = Regex.Replace(input, @"[^\d]", "");

            if (string.IsNullOrWhiteSpace(numbersOnly))
                return "";

            // Pad with zeros to 4 digits
            return numbersOnly.PadLeft(PaddedLength, '0');
        }

        /// <summary>
        /// Format RM number to "A.xxxx" format for output/display/submission
        /// Input: "32" or "A.32" or "A.0032" or "0032"
        /// Output: "A.0032" (always with padding)
        /// </summary>
        public static string FormatRmForOutput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            string numberPart = ExtractRmNumber(input);

            if (string.IsNullOrWhiteSpace(numberPart))
                return "";

            return $"{RmPrefix}.{numberPart}";
        }

        /// <summary>
        /// Validate if input is a valid RM format
        /// </summary>
        public static bool IsValidRmFormat(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            string extracted = ExtractRmNumber(input);

            // Should have at least some digits and be numeric
            return !string.IsNullOrWhiteSpace(extracted) && Regex.IsMatch(extracted, @"^\d+");
        }

        /// <summary>
        /// Get the search value (padded number) for database lookup
        /// Input: "32" → Output: "0032"
        /// </summary>
        public static string GetSearchValue(string input)
        {
            return ExtractRmNumber(input);
        }
    }
}

