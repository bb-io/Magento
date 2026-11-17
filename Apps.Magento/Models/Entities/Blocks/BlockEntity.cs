using Apps.Magento.Utils;
using Newtonsoft.Json;

namespace Apps.Magento.Models.Entities.Blocks;

public class BlockEntity
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonProperty("identifier")]
    public string Identifier { get; set; } = string.Empty;
    
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;
    
    [JsonProperty("content")]
    public string? Content { get; set; }
    
    [JsonProperty("creation_time"), JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime CreationTime { get; set; }
    
    [JsonProperty("update_time"), JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime UpdateTime { get; set; }
    
    [JsonProperty("active")]
    public bool Active { get; set; }
}