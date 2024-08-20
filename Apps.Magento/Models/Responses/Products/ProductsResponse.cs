using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Models.Responses.Products;

public class ProductsResponse : PaginationResponse<ProductResponse>
{
    [Display("Products")]
    public override List<ProductResponse> Items { get; set; } = new();
}
