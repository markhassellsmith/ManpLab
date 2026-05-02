using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;

namespace ManpWinUI.Converters;

/// <summary>
/// Converts a boolean value to a button style - AccentButtonStyle when true, default when false.
/// Used for resolution selector buttons to highlight the currently selected resolution.
/// </summary>
public class BoolToButtonStyleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, string language)
    {
        if (value is bool isSelected && isSelected)
        {
            // Return the AccentButtonStyle when true
            return Application.Current.Resources["AccentButtonStyle"] as Style;
        }

        // Return null for default style when false
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, string language)
    {
        throw new NotImplementedException();
    }
}
