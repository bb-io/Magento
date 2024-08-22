using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Models.Requests.Pages;

public class CreatePageRequest
{
    [Display("Identifier")] 
    public string Identifier { get; set; } = string.Empty;

    [Display("Title")] 
    public string Title { get; set; } = string.Empty;

    [Display("Page layout")] 
    public string? PageLayout { get; set; } = string.Empty;

    [Display("Meta title")] 
    public string? MetaTitle { get; set; } = string.Empty;

    [Display("Meta keywords")]
    public string? MetaKeywords { get; set; } = string.Empty;

    [Display("Meta description")] 
    public string? MetaDescription { get; set; } = string.Empty;

    [Display("Content heading")] 
    public string? ContentHeading { get; set; } = string.Empty;

    [Display("Content")] 
    public string Content { get; set; } = string.Empty;

    [Display("Sort order")] 
    public string? SortOrder { get; set; } = "0";
}