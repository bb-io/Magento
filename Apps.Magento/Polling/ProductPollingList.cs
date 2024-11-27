using System.Text;
using Apps.Magento.Api;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Requests;
using Apps.Magento.Models.Responses.Products;
using Apps.Magento.Polling.Models;
using Apps.Magento.Polling.Models.Requests;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using Newtonsoft.Json;
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
        
        var products = await GetProducts(new BaseFilterRequest
        {
            CreatedAt = request.Memory.LastInteractionDate, 
            Title = filterRequest.Title
        }, filterRequest.CategoryIds?.ToList());
        
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

        var products = await GetProducts(new BaseFilterRequest
        {
            UpdatedAt = request.Memory.LastInteractionDate, 
            Title = filterRequest.Title
        }, filterRequest.CategoryIds?.ToList());
        
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

    private async Task<ProductsResponse> GetProducts(BaseFilterRequest request, List<string>? requestedCategoryIds)
    {
        var queryString = BuildQueryString(request);
        var requestUrl = $"/rest/V1/products?searchCriteria{queryString}";
        var products = await Client.ExecuteWithErrorHandling<ProductsResponse>(new ApiRequest(requestUrl, Method.Get, Creds));
        
        if (requestedCategoryIds != null)
        {
            var result = new List<ProductResponse>();
            foreach (var productResponse in products.Items)
            {
                var categoriesCustomAttribute = productResponse.CustomAttributes.FirstOrDefault(x => x.AttributeCode == "category_ids");
                if (categoriesCustomAttribute != null)
                {
                    var categoryIds = JsonConvert.DeserializeObject<List<string>>(categoriesCustomAttribute.Value)!;
                    if (requestedCategoryIds.All(id => categoryIds.Contains(id)))
                    {
                        result.Add(productResponse);
                    }
                }
            }
            
            products.Items = result;
            products.TotalCount = result.Count;
        }
        
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