using System.Globalization;

internal static class TextHelper
{
    public static string Format(double value)
    {
        return value.ToString("F1", CultureInfo.InvariantCulture);
    }
}