using System.Text;
using Apps.Magento.Api;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Requests;
using Apps.Magento.Models.Responses.Pages;
using Apps.Magento.Polling.Models;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;

namespace Apps.Magento.Polling;

[PollingEventList]
public class PagePollingList(InvocationContext invocationContext) : AppInvocable(invocationContext)
{
    [PollingEvent("On pages created", "Triggered when new pages are created")]
    public async Task<PollingEventResponse<DateMemory, PagesResponse>> OnPagesCreated(
        PollingEventRequest<DateMemory> request,
        [PollingEventParameter] StoreViewOptionalIdentifier storeViewIdentifier)
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
        
        var pages = await GetPages(new BaseFilterRequest { CreatedAt = request.Memory.LastInteractionDate }, storeViewIdentifier.ToString());
        return new()
        {
            FlyBird = pages.Items.Any(),
            Result = pages,
            Memory = new()
            {
                LastInteractionDate = DateTime.UtcNow
            }
        };
    }
    
    [PollingEvent("On pages updated", "Triggered when pages are updated")]
    public async Task<PollingEventResponse<DateMemory, PagesResponse>> OnPagesUpdated(
        PollingEventRequest<DateMemory> request,
        [PollingEventParameter] StoreViewOptionalIdentifier storeViewIdentifier)
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
        
        var pages = await GetPages(new BaseFilterRequest { UpdatedAt = request.Memory.LastInteractionDate }, storeViewIdentifier.ToString());
        return new()
        {
            FlyBird = pages.Items.Any(),
            Result = pages,
            Memory = new()
            {
                LastInteractionDate = DateTime.UtcNow
            }
        };
    }
    
    private async Task<PagesResponse> GetPages(BaseFilterRequest filter, string storeView)
    {
        var queryString = BuildQueryString(filter);
        var requestUrl = $"/rest/{storeView}/V1/cmsPage/search?searchCriteria{queryString}";
        return await Client.ExecuteWithErrorHandling<PagesResponse>(new ApiRequest(requestUrl, Method.Get, Creds));
    }
    
    protected override string BuildQueryString(BaseFilterRequest filterRequest)
    {
        var queryString = new StringBuilder();
        var filterIndex = 0;
        
        if (filterRequest.CreatedAt.HasValue)
        {
            queryString.Append($"[filterGroups][0][filters][{filterIndex}][field]={Uri.EscapeDataString("creation_time")}");
            queryString.Append(
                $"&searchCriteria[filterGroups][0][filters][{filterIndex}][value]={Uri.EscapeDataString(filterRequest.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss"))}");
            queryString.Append(
                $"&searchCriteria[filterGroups][0][filters][{filterIndex}][conditionType]={Uri.EscapeDataString("gt")}");
            
            filterIndex += 1;
        }

        if (filterRequest.UpdatedAt.HasValue)
        {
            queryString.Append($"[filterGroups][0][filters][{filterIndex}][field]={Uri.EscapeDataString("update_time")}");
            queryString.Append(
                $"&searchCriteria[filterGroups][0][filters][{filterIndex}][value]={Uri.EscapeDataString(filterRequest.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss"))}");
            queryString.Append(
                $"&searchCriteria[filterGroups][0][filters][{filterIndex}][conditionType]={Uri.EscapeDataString("gt")}");
            
            filterIndex += 1;
        }

        return queryString.ToString();
    }
}