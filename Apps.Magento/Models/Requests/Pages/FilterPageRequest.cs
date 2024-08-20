using Apps.Magento.DataSources.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Magento.Models.Requests.Pages;

public class FilterPageRequest
{
    [Display("Title", Description = "You can filter by title")]
    public string? Title { get; set; }

    [Display("Condition type"), StaticDataSource(typeof(ConditionTypeStaticDataSource))]
    public string? ConditionType { get; set; }
}