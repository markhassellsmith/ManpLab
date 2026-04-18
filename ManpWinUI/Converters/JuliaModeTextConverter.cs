using Microsoft.UI.Xaml.Data;
using System;

namespace ManpWinUI.Converters;

/// <summary>
/// Converts boolean Julia mode flag to display text.
/// </summary>
public class JuliaModeTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isJuliaMode)
        {
            return isJuliaMode ? "Julia Set" : "Mandelbrot Set";
        }
        return "Mandelbrot Set";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
