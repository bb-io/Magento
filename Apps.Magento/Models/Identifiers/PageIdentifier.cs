using Apps.Magento.DataSources;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Magento.Models.Identifiers;

public class PageIdentifier
{
    [Display("Page ID", Description = "The unique identifier of the page."), DataSource(typeof(PageDataSource))]
    public string PageId { get; set; } = string.Empty;
}