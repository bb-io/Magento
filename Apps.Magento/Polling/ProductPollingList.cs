using Apps.Magento.Invocables;
using Apps.Magento.Models.Entities.Products;
using Apps.Magento.Models.Identifiers.Optional;
using Apps.Magento.Models.Responses.Products;
using Apps.Magento.Polling.Models;
using Apps.Magento.Polling.Models.Requests;
using Apps.Magento.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;

namespace Apps.Magento.Polling;

[PollingEventList("Products")]
public class ProductPollingList(InvocationContext invocationContext) : AppInvocable(invocationContext)
{
    [MultipleEvents, PollingEvent("On product created", "Triggered when a product is created")]
    public async Task<PollingEventResponse<DateMemory, List<ProductResponse>>> OnProductCreated(
        PollingEventRequest<DateMemory> pollingRequest,
        [PollingEventParameter] StoreViewOptionalIdentifier storeViewIdentifier,
        [PollingEventParameter] ProductPollingRequest input)
    {
        var pollingStart = PollingEventResults.PollingStart;
        
        if (pollingRequest.Memory is null)
            return PollingEventResults.NoFlight<List<ProductResponse>>(pollingStart);
        
        RestRequest request = new SearchCriteriaBuilder()
            .WhereContains(("name", input.NameContains))
            .WhereGreaterThan(("created_at", pollingRequest.Memory.LastInteractionDate))
            .ApplyTo(new RestRequest($"{storeViewIdentifier}/V1/products"));

        var response = await Client.Paginate<ProductEntity>(request);
        if (input.CategoryIds != null)
            response = response.Where(x => x.IsInAllCategories(input.CategoryIds));

        var responseList = response.ToList();
        if (responseList.Count == 0)
            return PollingEventResults.NoFlight<List<ProductResponse>>(pollingStart);
        
        var products = responseList.Select(x => new ProductResponse(x)).ToList();
        return PollingEventResults.Flight(products, pollingStart);
    }
    
    [MultipleEvents, PollingEvent("On product updated", "Triggered when a product is updated")]
    public async Task<PollingEventResponse<DateMemory, List<ProductResponse>>> OnProductUpdated(
        PollingEventRequest<DateMemory> pollingRequest,
        [PollingEventParameter] StoreViewOptionalIdentifier storeViewIdentifier,
        [PollingEventParameter] ProductPollingRequest input)
    {
        var pollingStart = PollingEventResults.PollingStart;
        
        if (pollingRequest.Memory is null)
            return PollingEventResults.NoFlight<List<ProductResponse>>(pollingStart);
        
        RestRequest request = new SearchCriteriaBuilder()
            .WhereContains(("name", input.NameContains))
            .WhereGreaterThan(("updated_at", pollingRequest.Memory.LastInteractionDate))
            .ApplyTo(new RestRequest($"{storeViewIdentifier}/V1/products"));

        var response = await Client.Paginate<ProductEntity>(request);
        if (input.CategoryIds != null)
            response = response.Where(x => x.IsInAllCategories(input.CategoryIds));

        var responseList = response.Where(x => x.UpdatedAt > x.CreatedAt).ToList();
        if (responseList.Count == 0)
            return PollingEventResults.NoFlight<List<ProductResponse>>(pollingStart);
        
        var products = responseList.Select(x => new ProductResponse(x)).ToList();
        return PollingEventResults.Flight(products, pollingStart);
    }
}