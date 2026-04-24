using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace ManpWinUI.Converters;

/// <summary>
/// Converts an empty or null string to Visibility.Collapsed, otherwise Visibility.Visible.
/// </summary>
public class EmptyStringToCollapsedConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
