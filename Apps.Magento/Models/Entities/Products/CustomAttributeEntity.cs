using Apps.Magento.Utils;
using Newtonsoft.Json;

namespace Apps.Magento.Models.Entities.Products;

public class CustomAttributeEntity
{
    [JsonProperty("attribute_code")]
    public string AttributeCode { get; set; } = string.Empty;
     
    [JsonProperty("value"), JsonConverter(typeof(StringOrUndefinedConverter))]
    public string Value { get; set; } = string.Empty;
}