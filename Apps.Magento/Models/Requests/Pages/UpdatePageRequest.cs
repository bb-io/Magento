using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Models.Requests.Pages;

public class UpdatePageRequest
{
    [Display("Page identifier")] 
    public string? Identifier { get; set; } 

    [Display("Page title")] 
    public string? Title { get; set; }

    [Display("Page meta title")] 
    public string? MetaTitle { get; set; }

    [Display("Page meta keywords")]
    public string? MetaKeywords { get; set; }

    [Display("Page meta description")] 
    public string? MetaDescription { get; set; }

    [Display("Page content heading")] 
    public string? ContentHeading { get; set; }
    
    [Display("Page is active")]
    public bool? Active { get; set; }
}