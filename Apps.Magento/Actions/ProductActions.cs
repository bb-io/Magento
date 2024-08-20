using Apps.Magento.Api;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Responses.Products;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Magento.Actions;

[ActionList]
public class ProductActions(InvocationContext invocationContext) : AppInvocable(invocationContext)
{
    [Action("Get all products", Description = "Get all products")]
    public async Task<ProductsResponse> GetAllProductsAsync()
    {
        return await Client.ExecuteWithErrorHandling<ProductsResponse>(new ApiRequest("/rest/V1/products?searchCriteria", Method.Get, Creds));
    }
}