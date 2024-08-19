using Apps.Magento.Models.Dtos;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Models.Responses.Products;

public class GetAllProductsResponse
{
    [Display("Products")]
    public List<ProductResponse> Items { get; set; } = new();
    
    [DefinitionIgnore]
    public SearchCriteriaDto SearchCriteria { get; set; } = new();
    
    [Display("Total count")]
    public int TotalCount { get; set; }
}
