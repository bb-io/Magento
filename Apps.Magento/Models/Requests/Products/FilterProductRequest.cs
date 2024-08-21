using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Models.Requests.Products;

public class FilterProductRequest : BaseFilterRequest
{    
    [Display("Name", Description = "You can filter by name")]
    public override string? Title { get; set; }
}