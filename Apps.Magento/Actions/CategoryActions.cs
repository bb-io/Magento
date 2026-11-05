using Apps.Magento.Extensions;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Entities.Categories;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Requests.Categories;
using Apps.Magento.Models.Responses.Categories;
using Apps.Magento.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Magento.Actions;

[ActionList("Categories")]
public class CategoryActions(InvocationContext invocationContext) : AppInvocable(invocationContext)
{
    [Action("Search categories", Description = "Search categories by specific criteria")]
    public async Task<SearchCategoriesResponse> SearchCategories([ActionParameter] SearchCategoriesRequest input)
    {
        RestRequest request = new SearchCriteriaBuilder()
            .WhereContains(("name", input.NameContains))
            .WhereLessThan(("created_at", input.CreatedBefore))
            .WhereGreaterThan(("created_at", input.CreatedAfter))
            .WhereLessThan(("updated_at", input.UpdatedBefore))
            .WhereGreaterThan(("updated_at", input.UpdatedAfter))
            .ApplyTo(new RestRequest("V1/categories/list"));
        
        var response = await Client.Paginate<CategoryEntity>(request);
        var categories = response.Select(x => new CategoryResponse(x)).ToList();
        return new(categories);
    }
    
    [Action("Get category", Description = "Get category by ID")]
    public async Task<CategoryResponse> GetCategory([ActionParameter] CategoryIdentifier categoryIdentifier)
    {
        var category = await Client.GetCategory(categoryIdentifier.CategoryId);
        return new(category);
    }
}