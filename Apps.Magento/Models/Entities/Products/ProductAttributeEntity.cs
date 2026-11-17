using Newtonsoft.Json;

namespace Apps.Magento.Models.Entities.Products;

public class ProductAttributeEntity
{
    [JsonProperty("attribute_code")]
    public string AttributeCode { get; set; } = string.Empty;

    [JsonProperty("default_frontend_label")]
    public string DefaultFrontendLabel { get; set; } = string.Empty;
}