using Apps.Magento.DataSources;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Magento.Models.Requests.Products;

public class FilterProductRequest : BaseFilterRequest
{    
    [Display("Name", Description = "You can filter by name")]
    public override string? Title { get; set; }
    
    [Display("Category IDs", Description = "Filter products that belong to all specified category IDs"), DataSource(typeof(CategoryDataHandler))]
    public IEnumerable<string>? CategoryIds { get; set; }
}