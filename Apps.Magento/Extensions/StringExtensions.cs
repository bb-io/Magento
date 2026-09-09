namespace Apps.Magento.Extensions;

public static class StringExtensions
{
    public static string SanitizeCurlyBraces(this string source)
    {
        return source.Replace("{", "{{").Replace("}", "}}").Trim();
    }
}