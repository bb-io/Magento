using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Magento.Models.Requests.Pages;

public class CreatePageRequest
{
    [Display("Page identifier")] 
    public string Identifier { get; set; } = string.Empty;

    [Display("Page title")] 
    public string Title { get; set; } = string.Empty;

    [Display("File", Description = "File with the page content")]
    public FileReference? File { get; set; }
    
    [Display("Page meta title")] 
    public string? MetaTitle { get; set; }

    [Display("Page meta keywords")]
    public string? MetaKeywords { get; set; }

    [Display("Page meta description")] 
    public string? MetaDescription { get; set; }

    [Display("Page content heading")] 
    public string? ContentHeading { get; set; }

    [Display("Page is active", Description = "True by default")]
    public bool? Active { get; set; }
}