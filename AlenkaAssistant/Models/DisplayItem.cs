namespace AlenkaAssistant.Models
{
    /// <summary>
    /// Generic class to wrap an enum value with its display name for UI binding.
    /// </summary>
    public class DisplayItem<T> where T : struct, System.Enum
    {
        public T Value { get; set; }
        public string DisplayName { get; set; }

        public DisplayItem(T value, string displayName)
        {
            Value = value;
            DisplayName = displayName;
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public override bool Equals(object obj)
        {
            if (obj is DisplayItem<T> other)
            {
                return Value.Equals(other.Value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }

    /// <summary>
    /// Simple string-based display item for configurable dropdowns
    /// </summary>
    public class DisplayItemString
    {
        public string Value { get; set; }
        public string DisplayName { get; set; }

        public DisplayItemString(string value, string displayName = null)
        {
            Value = value;
            DisplayName = displayName ?? value;
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public override bool Equals(object obj)
        {
            if (obj is DisplayItemString other)
            {
                return Value.Equals(other.Value, System.StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
