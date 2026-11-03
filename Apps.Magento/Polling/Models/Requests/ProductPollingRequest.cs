using Apps.Magento.DataSources;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Magento.Polling.Models.Requests;

public class ProductPollingRequest
{
    [Display("Name contains")]
    public string? NameContains { get; set; }
    
    [Display("Category IDs", Description = "Filter products that belong to all specified category IDs")] 
    [DataSource(typeof(CategoryDataSource))]
    public IEnumerable<string>? CategoryIds { get; set; }
}