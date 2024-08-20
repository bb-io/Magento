using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Models.Responses.Pages;

public class PageResponse
{
    [Display("Page ID")]
    public string Id { get; set; } = string.Empty;
    
    [Display("Identifier")]
    public string Identifier { get; set; } = string.Empty;
    
    [Display("Title")]
    public string Title { get; set; } = string.Empty;
    
    [Display("Page layout")]
    public string PageLayout { get; set; } = string.Empty;
    
    [Display("Meta title")]
    public string MetaTitle { get; set; } = string.Empty;
    
    [Display("Meta keywords")]
    public string MetaKeywords { get; set; } = string.Empty;
    
    [Display("Meta description")]
    public string MetaDescription { get; set; } = string.Empty;
    
    [Display("Content heading")]
    public string ContentHeading { get; set; } = string.Empty;
    
    [Display("Content")]
    public string Content { get; set; } = string.Empty;
    
    [Display("Creation time")]
    public DateTime CreationTime { get; set; }
    
    [Display("Update time")]
    public DateTime UpdateTime { get; set; }
    
    [Display("Sort order")]
    public string SortOrder { get; set; } = string.Empty;
    
    [Display("Layout update XML")]
    public string LayoutUpdateXml { get; set; } = string.Empty;
    
    [Display("Custom theme")]
    public string CustomTheme { get; set; } = string.Empty;
    
    [Display("Custom root template")]
    public string CustomRootTemplate { get; set; } = string.Empty;
    
    [Display("Active")]
    public bool Active { get; set; }
}