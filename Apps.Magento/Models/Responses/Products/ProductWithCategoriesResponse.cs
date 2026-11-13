using Apps.Magento.Models.Entities.Products;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Models.Responses.Products;

public class ProductWithCategoriesResponse(ProductEntity entity, List<string> categoryNames) : ProductResponse(entity)
{
    [Display("Category names")]
    public List<string> CategoryNames { get; set; } = categoryNames;
}