using Apps.Magento.Extensions;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Responses.Categories;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Magento.Actions;

[ActionList("Category")]
public class CategoryActions(InvocationContext invocationContext) : AppInvocable(invocationContext)
{
    [Action("Search categories", Description = "Retrieve all categories that match the specified criteria")]
    public async Task<CategoriesResponse> GetAllCategoriesAsync()
    {
        var request = new RestRequest("V1/categories/list?searchCriteria");
        return await Client.ExecuteWithErrorHandling<CategoriesResponse>(request);
    }
    
    [Action("Get category", Description = "Get category by ID")]
    public async Task<CategoryResponse> GetCategory([ActionParameter] CategoryIdentifier categoryIdentifier)
    {
        var category = await Client.GetCategory(categoryIdentifier.CategoryId);
        return new(category);
    }
}