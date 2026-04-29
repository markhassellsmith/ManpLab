using Microsoft.UI.Xaml.Data;
using System;

namespace ManpWinUI.Converters;

/// <summary>
/// Converts a boolean paused state to an opacity value.
/// Returns 0.5 when paused (true), 1.0 when not paused (false).
/// </summary>
public class PausedToOpacityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isPaused)
        {
            return isPaused ? 0.5 : 1.0;
        }
        return 1.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
