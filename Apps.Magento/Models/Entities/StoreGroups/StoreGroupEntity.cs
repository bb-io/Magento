using Newtonsoft.Json;

namespace Apps.Magento.Models.Entities.StoreGroups;

public class StoreGroupEntity
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
}