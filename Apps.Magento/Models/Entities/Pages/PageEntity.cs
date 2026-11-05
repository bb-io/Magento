using Apps.Magento.Utils;
using Newtonsoft.Json;

namespace Apps.Magento.Models.Entities.Pages;

public class PageEntity
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonProperty("identifier")]
    public string Identifier { get; set; } = string.Empty;
    
    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;
    
    [JsonProperty("page_layout")]
    public string? PageLayout { get; set; }
    
    [JsonProperty("meta_title")]
    public string? MetaTitle { get; set; }
    
    [JsonProperty("meta_keywords")]
    public string? MetaKeywords { get; set; }
    
    [JsonProperty("meta_description")]
    public string? MetaDescription { get; set; }
    
    [JsonProperty("content_heading")]
    public string? ContentHeading { get; set; }
    
    [JsonProperty("content")]
    public string? Content { get; set; }
    
    [JsonProperty("creation_time"), JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime CreationTime { get; set; }
    
    [JsonProperty("update_time"), JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime UpdateTime { get; set; }
    
    [JsonProperty("sort_order")]
    public string? SortOrder { get; set; }
    
    [JsonProperty("custom_theme")]
    public string? CustomTheme { get; set; }
    
    [JsonProperty("custom_root_template")]
    public string? CustomRootTemplate { get; set; }
    
    [JsonProperty("active")]
    public bool Active { get; set; }
}