using Newtonsoft.Json;

namespace Apps.Magento.Models.Entities.Attributes;

public class AttributeSetEntity
{
    [JsonProperty("attribute_set_id")]
    public int AttributeSetId { get; set; }
    
    [JsonProperty("attribute_set_name")]
    public string AttributeSetName { get; set; } = string.Empty;
    
    [JsonProperty("sort_order")]
    public int SortOrder { get; set; }
    
    [JsonProperty("entity_type_id")]
    public int EntityTypeId { get; set; }
}