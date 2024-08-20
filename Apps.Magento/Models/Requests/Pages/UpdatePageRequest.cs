using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Models.Requests.Pages;

public class UpdatePageRequest
{
    [Display("Identifier")] 
    public string? Identifier { get; set; } 

    [Display("Title")] 
    public string? Title { get; set; }

    [Display("Page layout")] 
    public string? PageLayout { get; set; }

    [Display("Meta title")] 
    public string? MetaTitle { get; set; }

    [Display("Meta keywords")]
    public string? MetaKeywords { get; set; }

    [Display("Meta description")] 
    public string? MetaDescription { get; set; }

    [Display("Content heading")] 
    public string? ContentHeading { get; set; }

    [Display("Content")] 
    public string Content { get; set; }

    [Display("Sort order")] 
    public string? SortOrder { get; set; } 
    
    [Display("Active")]
    public bool? Active { get; set; }
}