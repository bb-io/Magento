using Apps.Magento.Extensions;
using Newtonsoft.Json;

namespace Apps.Magento.Models.Dtos;

public class CustomAttributeDto(string attributeCode, string value)
{
    [JsonProperty("attribute_code")]
    public string AttributeCode { get; } = attributeCode;

    [JsonProperty("value")]
    public object Value { get; } = value.IsArrayString()
        ? JsonConvert.DeserializeObject<List<string>>(value)!
        : value;
}