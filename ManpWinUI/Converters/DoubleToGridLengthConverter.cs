using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace ManpWinUI.Converters;

/// <summary>
/// Converts a double value to a GridLength for column/row width binding.
/// </summary>
public class DoubleToGridLengthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is double doubleValue)
        {
            return new GridLength(doubleValue);
        }
        return new GridLength(1, GridUnitType.Auto);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is GridLength gridLength && gridLength.GridUnitType == GridUnitType.Pixel)
        {
            return gridLength.Value;
        }
        return 0.0;
    }
}
