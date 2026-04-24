using Microsoft.UI.Xaml.Data;
using System;

namespace ManpWinUI.Converters;

/// <summary>
/// Converts a boolean value to its inverse.
/// When targetType is Visibility, returns Visibility enum instead of bool.
/// </summary>
public class BoolNegationConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool boolValue)
        {
            bool negated = !boolValue;

            // If target type is Visibility, convert the negated bool to Visibility
            if (targetType == typeof(Visibility))
            {
                return negated ? Visibility.Visible : Visibility.Collapsed;
            }

            return negated;
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is Visibility visibility)
        {
            bool isVisible = visibility == Visibility.Visible;
            return !isVisible;
        }

        if (value is bool boolValue)
        {
            return !boolValue;
        }
        return value;
    }
}
