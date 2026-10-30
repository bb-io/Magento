using Newtonsoft.Json;

namespace Apps.Magento.Models.Entities.Categories;

public class CategoryEntity
{
    [JsonProperty("id")] 
    public string Id { get; set; } = string.Empty;
    
    [JsonProperty("parent_id")]
    public string ParentId { get; set; } = string.Empty;

    [JsonProperty("name")] 
    public string Name { get; set; } = string.Empty;

    [JsonProperty("is_active")]
    public bool IsActive { get; set; }
}