using Apps.Magento.DataSources.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Magento.Models.Requests;

public class BaseFilterRequest
{
    [Display("Title", Description = "You can filter by title")]
    public virtual string? Title { get; set; }

    [Display("Condition type"), StaticDataSource(typeof(ConditionTypeStaticDataSource))]
    public virtual string? ConditionType { get; set; }
}