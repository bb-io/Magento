using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Polling.Models.Requests;

public class OnContentCreatedOrUpdatedRequest
{
    [Display("Content name contains")]
    public string? NameContains { get; set; }
}