using Newtonsoft.Json;

namespace Apps.Magento.Models.Entities.StoreViews;

public class StoreViewEntity
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonProperty("code")]
    public string Code { get; set; } = string.Empty;
    
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty("is_active")]
    public int IsActive { get; set; }
}