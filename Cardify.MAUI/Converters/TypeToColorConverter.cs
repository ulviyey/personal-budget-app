using System.Globalization;

namespace Cardify.MAUI.Converters;

public class TypeToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string type)
        {
            return type.ToLower() switch
            {
                "income" => Color.FromArgb("#059669"), // Green
                "expense" => Color.FromArgb("#DC2626"), // Red
                "debt" => Color.FromArgb("#F59E0B"),    // Orange
                _ => Color.FromArgb("#6B7280")          // Gray
            };
        }
        return Color.FromArgb("#6B7280");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 