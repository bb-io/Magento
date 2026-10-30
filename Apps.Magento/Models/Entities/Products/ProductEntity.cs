using Apps.Magento.Utils;
using Newtonsoft.Json;

namespace Apps.Magento.Models.Entities.Products;

public class ProductEntity
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonProperty("sku")]
    public string Sku { get; set; } = string.Empty;
    
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty("attribute_set_id")]
    public string AttributeSetId { get; set; } = string.Empty;
    
    [JsonProperty("price")]
    public double Price { get; set; }
    
    [JsonProperty("status")]
    public int Status { get; set; }
    
    [JsonProperty("visibility")]
    public int Visibility { get; set; }
    
    [JsonProperty("type_id")]
    public string TypeId { get; set; } = string.Empty;
    
    [JsonProperty("created_at"), JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime CreatedAt { get; set; }
    
    [JsonProperty("updated_at"), JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime UpdatedAt { get; set; }
    
    [JsonProperty("weight")]
    public double Weight { get; set; }
    
    [JsonProperty("extension_attributes")]
    public ExtensionAttributes ExtensionAttributes { get; set; } = new();
    
    [JsonProperty("custom_attributes")]
    public List<CustomAttribute> CustomAttributes { get; set; } = new();
    
    public bool IsInAllCategories(IEnumerable<string> categoryIds)
    {
        var attribute = CustomAttributes.FirstOrDefault(x => x.AttributeCode == "category_ids");
        if (attribute is null)
            return false;

        var productCategoryIds = JsonConvert.DeserializeObject<List<string>>(attribute.Value)!;
        return categoryIds.All(productCategoryIds.Contains);
    }
}