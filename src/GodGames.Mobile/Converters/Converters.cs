using System.Globalization;

namespace GodGames.Mobile.Converters;

/// Converts bool (IsBusy) to one of two strings: "BUSY_TEXT|NORMAL_TEXT"
public class BusyToTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var isBusy = value is bool b && b;
        var parts = (parameter as string ?? "|").Split('|');
        return isBusy ? parts[0] : (parts.Length > 1 ? parts[1] : "");
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}

/// Converts string to bool (true when not null/empty)
public class StringNotEmptyConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        !string.IsNullOrEmpty(value as string);
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}

/// Inverts a bool
public class InverseBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is bool b && !b;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is bool b && !b;
}

/// Converts a percent (0.0–1.0) to a pixel width. Needs screen width — returns 0 as fallback.
public class PercentToWidthConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not double pct) return 0d;
        var screenWidth = DeviceDisplay.Current.MainDisplayInfo.Width / DeviceDisplay.Current.MainDisplayInfo.Density;
        return screenWidth * Math.Clamp(pct, 0, 1);
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}

/// True when value is not null
public class IsNotNullConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is not null;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}

/// True when integer value is zero (for empty state visibility)
public class ZeroToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is int i && i == 0;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}
