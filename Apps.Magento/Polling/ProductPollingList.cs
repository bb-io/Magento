using Apps.Magento.Api;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Requests;
using Apps.Magento.Models.Responses.Products;
using Apps.Magento.Polling.Models;
using Apps.Magento.Polling.Models.Requests;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;

namespace Apps.Magento.Polling;

[PollingEventList]
public class ProductPollingList(InvocationContext invocationContext) : AppInvocable(invocationContext)
{
    [PollingEvent("On products created", "Triggered when new products are created")]
    public async Task<PollingEventResponse<DateMemory, ProductsResponse>> OnProductsCreated(
        PollingEventRequest<DateMemory> request,
        [PollingEventParameter] OnProductsUpdatedRequest filterRequest)
    {
        if (request.Memory is null)
        {
            return new()
            {
                FlyBird = false,
                Memory = new()
                {
                    LastInteractionDate = DateTime.UtcNow
                }
            };
        }
        
        var products = await GetProducts(new BaseFilterRequest { CreatedAt = request.Memory.LastInteractionDate, Title = filterRequest.Title });
        return new()
        {
            FlyBird = products.Items.Any(),
            Result = products,
            Memory = new()
            {
                LastInteractionDate = DateTime.UtcNow
            }
        };
    }
    
    [PollingEvent("On products updated", "Triggered when products are updated")]
    public async Task<PollingEventResponse<DateMemory, ProductsResponse>> OnProductsUpdated(
        PollingEventRequest<DateMemory> request,
        [PollingEventParameter] OnProductsUpdatedRequest filterRequest)
    {
        if (request.Memory is null)
        {
            return new()
            {
                FlyBird = false,
                Memory = new()
                {
                    LastInteractionDate = DateTime.UtcNow
                }
            };
        }

        var products = await GetProducts(new BaseFilterRequest { UpdatedAt = request.Memory.LastInteractionDate, Title = filterRequest.Title });
        products.Items = products.Items.Where(x => x.CreatedAt != x.UpdatedAt).ToList();

        return new()
        {
            FlyBird = products.Items.Any(),
            Result = products,
            Memory = new()
            {
                LastInteractionDate = DateTime.UtcNow
            }
        };
    }

    private async Task<ProductsResponse> GetProducts(BaseFilterRequest request)
    {
        var queryString = BuildQueryString(request);
        var requestUrl = $"/rest/V1/products?searchCriteria{queryString}";
        return await Client.ExecuteWithErrorHandling<ProductsResponse>(new ApiRequest(requestUrl, Method.Get, Creds));
    }
}