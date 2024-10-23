using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Models.Responses.Categories;


public class CategoriesResponse : PaginationResponse<CategoryResponse>
{
    [Display("Categories")] 
    public override List<CategoryResponse> Items { get; set; } = new();
}