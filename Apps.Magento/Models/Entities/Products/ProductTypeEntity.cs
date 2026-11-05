using Newtonsoft.Json;

namespace Apps.Magento.Models.Entities.Products;

public class ProductTypeEntity
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty("label")]
    public string Label { get; set; } = string.Empty;
}