using Apps.Magento.Models.Entities.Pages;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Models.Responses.Pages;

public class PageResponse(PageEntity entity)
{
    [Display("Page ID")]
    public string Id { get; set; } = entity.Id;

    [Display("Page identifier")]
    public string Identifier { get; set; } = entity.Identifier;

    [Display("Page title")]
    public string Title { get; set; } = entity.Title;

    [Display("Page layout")]
    public string? PageLayout { get; set; } = entity.PageLayout;

    [Display("Page meta title")]
    public string? MetaTitle { get; set; } = entity.MetaTitle;

    [Display("Page meta keywords")]
    public string? MetaKeywords { get; set; } = entity.MetaKeywords;

    [Display("Page meta description")]
    public string? MetaDescription { get; set; } = entity.MetaDescription;

    [Display("Page content heading")]
    public string? ContentHeading { get; set; } = entity.ContentHeading;

    [Display("Page created at")]
    public DateTime CreatedAt { get; set; } = entity.CreationTime;

    [Display("Page updated at")]
    public DateTime UpdatedAt { get; set; } = entity.UpdateTime;

    [Display("Page sort order")]
    public string? SortOrder { get; set; } = entity.SortOrder;

    [Display("Page is active")]
    public bool Active { get; set; } = entity.Active;
}