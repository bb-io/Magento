namespace Apps.Magento.Extensions;

public static class StreamExtensions
{
    public static string ReadString(this Stream stream) => new StreamReader(stream).ReadToEnd();
}