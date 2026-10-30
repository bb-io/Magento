using Newtonsoft.Json;

namespace Apps.Magento.Models.Entities.Products;

public class ExtensionAttributes
{
    [JsonProperty("website_ids")]
    public List<int> WebsiteIds { get; set; } = new();
    
    [JsonProperty("category_links")]
    public List<CategoryLink> CategoryLinks { get; set; } = new();
}