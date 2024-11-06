using Apps.Magento.Api;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Responses.Categories;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Magento.Actions;

[ActionList]
public class CategoryActions(InvocationContext invocationContext) : AppInvocable(invocationContext)
{
    [Action("Search categories", Description = "Retrieve all categories that match the specified criteria")]
    public async Task<CategoriesResponse> GetAllCategoriesAsync(
        [ActionParameter] StoreViewOptionalIdentifier storeViewIdentifier)
    {
        var requestUrl = $"/rest/{storeViewIdentifier}/V1/categories/list";
        var request = new ApiRequest(requestUrl, Method.Get, Creds);
        return await Client.ExecuteWithErrorHandling<CategoriesResponse>(request);
    }
    
    [Action("Get category", Description = "Get category by ID")]
    public async Task<CategoryResponse> GetCategoryAsync(
        [ActionParameter] CategoryIdentifier categoryIdentifier,
        [ActionParameter] StoreViewOptionalIdentifier storeViewIdentifier)
    {
        var requestUrl = $"/rest/{storeViewIdentifier}/V1/categories/{categoryIdentifier.CategoryId}";
        var request = new ApiRequest(requestUrl, Method.Get, Creds);
        return await Client.ExecuteWithErrorHandling<CategoryResponse>(request);
    }
}