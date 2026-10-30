using System.Text;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Entities.Products;
using Apps.Magento.Models.Requests;
using Apps.Magento.Models.Responses.Products;
using Apps.Magento.Polling.Models;
using Apps.Magento.Polling.Models.Requests;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;

namespace Apps.Magento.Polling;

[PollingEventList]
public class ProductPollingList(InvocationContext invocationContext) : AppInvocable(invocationContext)
{
    [MultipleEvents, PollingEvent("On product created", "Triggered when new products are created")]
    public async Task<PollingEventResponse<DateMemory, List<ProductResponse>>> OnProductCreated(
        PollingEventRequest<DateMemory> request,
        [PollingEventParameter] OnProductsUpdatedRequest filterRequest)
    {
        var pollingStart = DateTime.UtcNow;
        
        if (request.Memory is null)
        {
            return new()
            {
                FlyBird = false,
                Memory = new() { LastInteractionDate = pollingStart }
            };
        }
        
        var products = await GetProducts(new BaseFilterRequest
        {
            CreatedAt = request.Memory.LastInteractionDate, 
            Title = filterRequest.Title
        }, filterRequest.CategoryIds?.ToList());
        var productsList = products.ToList();
        
        return new()
        {
            FlyBird = productsList.Count != 0,
            Result = productsList.Select(x => new ProductResponse(x)).ToList(),
            Memory = new() { LastInteractionDate = pollingStart }
        };
    }
    
    [PollingEvent("On product updated", "Triggered when products are updated")]
    public async Task<PollingEventResponse<DateMemory, List<ProductResponse>>> OnProductUpdated(
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

        var products = await GetProducts(new BaseFilterRequest
        {
            UpdatedAt = request.Memory.LastInteractionDate, 
            Title = filterRequest.Title
        }, filterRequest.CategoryIds?.ToList());

        products = products.Where(x => x.CreatedAt != x.UpdatedAt);
        var productsList = products.ToList();
        
        return new()
        {
            FlyBird = productsList.Count != 0,
            Result = productsList.Select(x => new ProductResponse(x)).ToList(),
            Memory = new()
            {
                LastInteractionDate = DateTime.UtcNow
            }
        };
    }

    private async Task<IEnumerable<ProductEntity>> GetProducts(BaseFilterRequest input, List<string>? requestedCategoryIds)
    {
        var queryString = BuildQueryString(input);
        var request = new RestRequest($"V1/products?searchCriteria{queryString}");
        var products = await Client.Paginate<ProductEntity>(request);
        
        if (requestedCategoryIds != null)
            products = products.Where(x => x.IsInAllCategories(requestedCategoryIds));
        
        return products;
    }
    
    protected override string BuildQueryString(BaseFilterRequest filterRequest)
    {
        var queryString = new StringBuilder();
        var filterIndex = 0;

        if (!string.IsNullOrEmpty(filterRequest.Title))
        {
            queryString.Append($"[filterGroups][{filterIndex}][filters][0][field]={Uri.EscapeDataString("name")}");
            queryString.Append(
                $"&searchCriteria[filterGroups][{filterIndex}][filters][0][value]={Uri.EscapeDataString($"%{filterRequest.Title}%")}");
            queryString.Append(
                $"&searchCriteria[filterGroups][{filterIndex}][filters][0][conditionType]={Uri.EscapeDataString("like")}");

            filterIndex += 1;
        }
        
        if (filterRequest.CreatedAt.HasValue)
        {
            queryString.Append($"[filterGroups][0][filters][{filterIndex}][field]={Uri.EscapeDataString("created_at")}");
            queryString.Append(
                $"&searchCriteria[filterGroups][0][filters][{filterIndex}][value]={Uri.EscapeDataString(filterRequest.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss"))}");
            queryString.Append(
                $"&searchCriteria[filterGroups][0][filters][{filterIndex}][conditionType]={Uri.EscapeDataString("gt")}");
            
            filterIndex += 1;
        }

        if (filterRequest.UpdatedAt.HasValue)
        {
            queryString.Append($"[filterGroups][0][filters][{filterIndex}][field]={Uri.EscapeDataString("updated_at")}");
            queryString.Append(
                $"&searchCriteria[filterGroups][0][filters][{filterIndex}][value]={Uri.EscapeDataString(filterRequest.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss"))}");
            queryString.Append(
                $"&searchCriteria[filterGroups][0][filters][{filterIndex}][conditionType]={Uri.EscapeDataString("gt")}");
            
            filterIndex += 1;
        }

        return queryString.ToString();
    }
}