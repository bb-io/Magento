using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Models.Requests.StoreViews;

public class SearchStoreViewsRequest
{
    [Display("Name contains")]
    public string? NameContains { get; set; }
}