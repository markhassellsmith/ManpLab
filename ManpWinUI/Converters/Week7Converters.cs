using Microsoft.UI.Xaml.Data;
using System;

namespace ManpWinUI.Converters
{
    /// <summary>
    /// Converts boolean to opacity (true = 1.0, false = 0.0).
    /// Week 7 Task 1: For palette selection highlighting.
    /// </summary>
    public class BoolToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolValue)
            {
                return boolValue ? 1.0 : 0.0;
            }
            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts enum value to boolean for RadioButton binding.
    /// Week 7 Task 1: For render mode selection.
    /// </summary>
    public class EnumToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || parameter == null)
                return false;

            return value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolValue && boolValue && parameter != null)
            {
                return Enum.Parse(targetType, parameter.ToString()!);
            }
            return value;
        }
    }

    /// <summary>
    /// Converts enum to integer index for ComboBox binding.
    /// Week 7 Task 1: For antialiasing level selection.
    /// </summary>
    public class EnumToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Enum enumValue)
            {
                return (int)(object)enumValue;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is int intValue)
            {
                return Enum.ToObject(targetType, intValue);
            }
            return value;
        }
    }
}
