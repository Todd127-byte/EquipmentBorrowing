using Avalonia.Data.Converters;
using System.Globalization;

namespace EquipmentBorrowing.Desktop.Converters;

public sealed class EquipmentAvailabilityConverter : IValueConverter
{
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        return value is bool isAvailable
            ? isAvailable ? "Available" : "Unavailable"
            : "Status unknown";
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        throw new NotSupportedException("Availability is displayed as text and cannot be edited here.");
    }
}
