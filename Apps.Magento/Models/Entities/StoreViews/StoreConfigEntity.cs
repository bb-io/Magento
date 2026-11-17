using Newtonsoft.Json;

namespace Apps.Magento.Models.Entities.StoreViews;

public class StoreConfigEntity
{
    [JsonProperty("code")]
    public string Code { get; set; } = string.Empty;

    [JsonProperty("locale")]
    public string Locale { get; set; } = string.Empty;
    
    [JsonProperty("secure_base_link_url")]
    public string BaseLinkUrl { get; set; } = string.Empty;

    public string Language => Locale.Replace('_', '-');
}