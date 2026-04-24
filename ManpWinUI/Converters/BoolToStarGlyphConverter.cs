using Microsoft.UI.Xaml.Data;
using System;

namespace ManpWinUI.Converters;

/// <summary>
/// Converts a boolean to a star glyph (filled or outline).
/// </summary>
public class BoolToStarGlyphConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isFavorite && isFavorite)
        {
            return "\uE735"; // Filled star
        }
        return "\uE734"; // Outline star
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
