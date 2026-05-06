using Microsoft.UI.Xaml.Data;
using System;

namespace ManpWinUI.Converters;

/// <summary>
/// Converts TimeSpan to formatted string (MM:SS or HH:MM:SS).
/// </summary>
public class TimeSpanToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        TimeSpan ts;

        // Handle both TimeSpan and TimeSpan?
        if (value is TimeSpan timeSpan)
        {
            ts = timeSpan;
        }
        else if (value != null && value.GetType() == typeof(TimeSpan?))
        {
            var nullable = (TimeSpan?)value;
            if (nullable.HasValue)
            {
                ts = nullable.Value;
            }
            else
            {
                return "--:--";
            }
        }
        else
        {
            return "--:--";
        }

        // Format as HH:MM:SS if hour >= 1, otherwise MM:SS
        if (ts.TotalHours >= 1)
        {
            return ts.ToString(@"hh\:mm\:ss");
        }
        else
        {
            return ts.ToString(@"mm\:ss");
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
