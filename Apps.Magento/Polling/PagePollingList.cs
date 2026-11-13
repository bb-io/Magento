using Apps.Magento.Invocables;
using Apps.Magento.Models.Entities.Pages;
using Apps.Magento.Models.Responses.Pages;
using Apps.Magento.Polling.Models;
using Apps.Magento.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;

namespace Apps.Magento.Polling;

[PollingEventList("Pages")]
public class PagePollingList(InvocationContext invocationContext) : AppInvocable(invocationContext)
{
    [MultipleEvents, PollingEvent("On page created", "Triggered when a new page is created")]
    public async Task<PollingEventResponse<DateMemory, List<PageResponse>>> OnPageCreated(
        PollingEventRequest<DateMemory> pollingRequest)
    {
        var pollingStart = PollingEventResults.PollingStart;
        
        if (pollingRequest.Memory is null)
            return PollingEventResults.NoFlight<List<PageResponse>>(pollingStart);
        
        RestRequest request = new SearchCriteriaBuilder()
            .WhereGreaterThan(("creation_time", pollingRequest.Memory.LastInteractionDate))
            .ApplyTo(new RestRequest("V1/cmsPage/search"));
        
        var response = await Client.Paginate<PageEntity>(request);
        var responseList = response.ToList();
        if (responseList.Count == 0)
            return PollingEventResults.NoFlight<List<PageResponse>>(pollingStart);
        
        var pages = responseList.Select(x => new PageResponse(x)).ToList();
        return PollingEventResults.Flight(pages, pollingStart);
    }
    
    [MultipleEvents, PollingEvent("On page updated", "Triggered when a page is updated")]
    public async Task<PollingEventResponse<DateMemory, List<PageResponse>>> OnPageUpdated(
        PollingEventRequest<DateMemory> pollingRequest)
    {
        var pollingStart = PollingEventResults.PollingStart;
        
        if (pollingRequest.Memory is null)
            return PollingEventResults.NoFlight<List<PageResponse>>(pollingStart);
        
        RestRequest request = new SearchCriteriaBuilder()
            .WhereGreaterThan(("update_time", pollingRequest.Memory.LastInteractionDate))
            .ApplyTo(new RestRequest("V1/cmsPage/search"));
        
        var response = await Client.Paginate<PageEntity>(request);
        var responseList = response.Where(x => x.UpdateTime > x.CreationTime).ToList();
        if (responseList.Count == 0)
            return PollingEventResults.NoFlight<List<PageResponse>>(pollingStart);
        
        var pages = responseList.Select(x => new PageResponse(x)).ToList();
        return PollingEventResults.Flight(pages, pollingStart);
    }
}