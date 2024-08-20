using Apps.Magento.Models.Dtos;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Models.Responses;

public class PaginationResponse<T>
{
    public virtual List<T> Items { get; set; } = new();
    
    [DefinitionIgnore]
    public SearchCriteriaDto SearchCriteria { get; set; } = new();
    
    [Display("Total count")]
    public int TotalCount { get; set; }
}