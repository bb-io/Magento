using Apps.Magento.DataSources;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Magento.Polling.Models.Requests;

public class OnProductsUpdatedRequest
{
    [Display("Title", Description = "By setting this parameter, the event will only trigger when the product title contains the specified value")]
    public string? Title { get; set; }
    
    [Display("Category IDs", Description = "Filter products that belong to all specified category IDs"), DataSource(typeof(CategoryDataHandler))]
    public IEnumerable<string>? CategoryIds { get; set; }
}