using System;
using System.Globalization;
using System.Windows.Data;

namespace SENTINEL.Converters;

public class ScoreToOffsetConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int score)
        {
            var circumference = 754.0; // 2 * π * r (r = 120)
            var offset = circumference - (circumference * score / 100.0);
            return offset;
        }
        return 754;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
