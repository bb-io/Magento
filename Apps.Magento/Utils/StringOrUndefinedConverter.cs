using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apps.Magento.Utils;

public class StringOrUndefinedConverter : JsonConverter<string>
{
    public override string ReadJson(JsonReader reader, Type objectType, string? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        try
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return "undefined";
            }

            if (reader.TokenType == JsonToken.StartArray)
            {
                var items = JArray.Load(reader);
                var stringItems = new List<string>();
                foreach (var item in items)
                {
                    stringItems.Add(item.ToString());
                }
                return $"[{string.Join(", ", stringItems)}]";
            }

            return reader.Value?.ToString() ?? "undefined";
        }
        catch
        {
            return "undefined";
        }
    }

    public override void WriteJson(JsonWriter writer, string? value, JsonSerializer serializer)
    {
        if (value.StartsWith("[") && value.EndsWith("]"))
        {
            var items = value.Substring(1, value.Length - 2)
                .Split(", ")
                .Select(item => int.TryParse(item, out var result) ? (object)result : item)
                .ToList();

            writer.WriteStartArray();
            foreach (var item in items)
            {
                writer.WriteValue(item);
            }
            writer.WriteEndArray();
        }
        else
        {
            writer.WriteValue(value);
        }
    }
}