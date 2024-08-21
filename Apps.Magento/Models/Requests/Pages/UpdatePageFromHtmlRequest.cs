using Apps.Magento.DataSources;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Magento.Models.Requests.Pages;

public class UpdatePageFromHtmlRequest
{
    [Display("Page ID", Description = "The unique identifier of the page."), DataSource(typeof(PageDataSource))]
    public string? PageId { get; set; }
 
    public FileReference File { get; set; } = new();
}