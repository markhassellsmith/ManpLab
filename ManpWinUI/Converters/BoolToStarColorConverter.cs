using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace ManpWinUI.Converters;

/// <summary>
/// Converts a boolean to a star color (gold for favorite, gray for non-favorite).
/// </summary>
public class BoolToStarColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isFavorite && isFavorite)
        {
            return new SolidColorBrush(Colors.Gold);
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
