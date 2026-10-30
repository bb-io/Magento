using Apps.Magento.DataSources;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Magento.Models.Requests.Products;

public class SearchProductsRequest
{
    [Display("Name")]
    public string? Name { get; set; }
    
    [Display("Created after")]
    public DateTime? CreatedAfter { get; set; }

    [Display("Created before")]
    public DateTime? CreatedBefore { get; set; }
    
    [Display("Updated after")]
    public DateTime? UpdatedAfter { get; set; }
    
    [Display("Updated before")]
    public DateTime? UpdatedBefore { get; set; }
    
    [Display("Category IDs"), DataSource(typeof(CategoryDataHandler))]
    public IEnumerable<string>? CategoryIds { get; set; }
}