using Newtonsoft.Json;

namespace Apps.Magento.Models.Entities.Products;

public class CategoryLink
{
    [JsonProperty("position")]
    public int Position { get; set; }
    
    [JsonProperty("category_id")]
    public string CategoryId { get; set; } = string.Empty;
}