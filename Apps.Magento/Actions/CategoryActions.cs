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
    [Action("Get all categories", Description = "Get all categories")]
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
    
    [Action("Get categories for product", Description = "Get categories for the specified product")]
    public async Task<CategoriesResponse> GetCategoriesForProductAsync(
        [ActionParameter] ProductIdentifier productIdentifier,
        [ActionParameter] StoreViewOptionalIdentifier storeViewIdentifier)
    {
        var productActions = new ProductActions(InvocationContext, null!);
        var product = await productActions.GetProductBySkuAsync(storeViewIdentifier, productIdentifier);
        
        var categories = new CategoriesResponse();
        
        foreach (var categoryLink in product.ExtensionAttributes.CategoryLinks)
        {
            var category = await GetCategoryAsync(new()
            {
                CategoryId = categoryLink.CategoryId
            }, storeViewIdentifier);
            categories.Items.Add(category);
        }
        
        return categories;
    }
}