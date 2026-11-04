using Apps.Magento.DataSources;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Magento.Models.Requests.Content;

public class SearchContentRequest
{
    [Display("Content name contains")]
    public string? NameContains { get; set; }
    
    [Display("Content created after")]
    public DateTime? CreatedAfter { get; set; }

    [Display("Content created before")]
    public DateTime? CreatedBefore { get; set; }
    
    [Display("Content updated after")]
    public DateTime? UpdatedAfter { get; set; }
    
    [Display("Content updated before")]
    public DateTime? UpdatedBefore { get; set; }
    
    [Display("Store view", Description = "Used to search products in a specific store view")]
    [DataSource(typeof(StoreViewDataSource))]
    public string? StoreView { get; set; }
    
    [Display("Product category IDs"), DataSource(typeof(CategoryDataSource))]
    public IEnumerable<string>? CategoryIds { get; set; }
}