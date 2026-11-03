namespace Apps.Magento.Extensions;

public static class StringExtensions
{
    public static string SanitizeCurlyBraces(this string source)
    {
        return source.Replace("{", "{{").Replace("}", "}}");
    }

    public static string Encode(this string value) => Uri.EscapeDataString(value);
    
    public static bool IsArrayString(this string value)
    {
        return value.StartsWith('[') && value.EndsWith(']');
    }
    
    public static bool Matches(this string text, string? search)
    {
        return string.IsNullOrEmpty(search) || text.Contains(search, StringComparison.OrdinalIgnoreCase);
    }
}