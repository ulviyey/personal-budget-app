using System.Globalization;

namespace Cardify.MAUI.Converters;

public class BoolToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isIncome)
        {
            return isIncome ? Color.FromArgb("#059669") : Color.FromArgb("#DC2626");
        }
        return Color.FromArgb("#6B7280");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 