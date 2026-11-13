using Apps.Magento.DataSources;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Magento.Models.Identifiers;

public class PageIdentifier
{
    [Display("Page ID"), DataSource(typeof(PageDataSource))]
    public string PageId { get; set; } = string.Empty;
}