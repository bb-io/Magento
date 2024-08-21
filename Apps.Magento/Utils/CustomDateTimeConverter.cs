using System.Globalization;
using Newtonsoft.Json;

namespace Apps.Magento.Utils;

public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    private readonly string[] _formats = { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-ddTHH:mm:ssZ" };

    public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return DateTime.MinValue;
        }

        var dateStr = reader.Value.ToString();

        if (string.IsNullOrEmpty(dateStr) || dateStr == "0000-00-00 00:00:00")
        {
            return DateTime.MinValue;
        }

        if (DateTime.TryParseExact(dateStr, _formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
        {
            return date;
        }

        throw new JsonReaderException($"Invalid date format: {dateStr}");
    }

    public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
    {
        if (value != DateTime.MinValue)
        {
            writer.WriteValue(value.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        else
        {
            writer.WriteNull();
        }
    }
}