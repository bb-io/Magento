using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Polling.Models.Requests;

public class OnProductsUpdatedRequest
{
    [Display("Title", Description = "By setting this parameter, the event will only trigger when the product title contains the specified value")]
    public string? Title { get; set; }
}