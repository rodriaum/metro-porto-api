namespace TransitGtfsApi.Utils;

public class NumberUtil
{
    public static int? ParseIntSafe(object? value, int? defaultValue = (int?)null)
    {
        if (value == null) return defaultValue;

        string? str = value.ToString();

        return string.IsNullOrWhiteSpace(str) ? defaultValue : int.Parse(str);
    }

    public static decimal? ParseDecimalSafe(object? value, decimal? defaultValue = (decimal?)null, IFormatProvider? format = null)
    {
        if (value == null) return defaultValue;

        string? str = value.ToString();

        return string.IsNullOrWhiteSpace(str)
            ? defaultValue
            : decimal.Parse(str, format);
    }

    public static double? ParseDoubleSafe(object? value, double? defaultValue = (double?)null, IFormatProvider? format = null)
    {
        if (value == null) return defaultValue;

        string? str = value.ToString();

        return string.IsNullOrWhiteSpace(str)
            ? defaultValue
            : double.Parse(str, format);
    }
}