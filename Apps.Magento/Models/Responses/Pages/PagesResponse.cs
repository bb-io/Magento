using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Models.Responses.Pages;

public class PagesResponse : PaginationResponse<PageResponse>
{
    [Display("Pages")]
    public override List<PageResponse> Items { get; set; } = new();
}