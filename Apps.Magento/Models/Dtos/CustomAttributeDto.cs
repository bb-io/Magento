using Newtonsoft.Json;

namespace Apps.Magento.Models.Dtos;

public class CustomAttributeDto(string attributeCode, object value)
{
    [JsonProperty("attribute_code")]
    public string AttributeCode { get; set; } = attributeCode;

    [JsonProperty("value")]
    public object Value { get; set; } = value;
}