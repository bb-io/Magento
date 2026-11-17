using Apps.Magento.Invocables;
using Apps.Magento.Models.Entities.Blocks;
using Apps.Magento.Models.Responses.Blocks;
using Apps.Magento.Polling.Models;
using Apps.Magento.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;

namespace Apps.Magento.Polling;

[PollingEventList("Blocks")]
public class BlockPollingList(InvocationContext invocationContext) : AppInvocable(invocationContext)
{
    [MultipleEvents, PollingEvent("On block created", "Triggered when a new block is created")]
    public async Task<PollingEventResponse<DateMemory, List<BlockResponse>>> OnBlockCreated(
        PollingEventRequest<DateMemory> pollingRequest)
    {
        var pollingStart = PollingEventResults.PollingStart;
        
        if (pollingRequest.Memory is null)
            return PollingEventResults.NoFlight<List<BlockResponse>>(pollingStart);
        
        RestRequest request = new SearchCriteriaBuilder()
            .WhereGreaterThan(("creation_time", pollingRequest.Memory.LastInteractionDate))
            .ApplyTo(new RestRequest("V1/cmsBlock/search"));
        
        var response = await Client.Paginate<BlockEntity>(request);
        var responseList = response.ToList();
        if (responseList.Count == 0)
            return PollingEventResults.NoFlight<List<BlockResponse>>(pollingStart);
        
        var blocks = responseList.Select(x => new BlockResponse(x)).ToList();
        return PollingEventResults.Flight(blocks, pollingStart);
    }
    
    [MultipleEvents, PollingEvent("On block updated", "Triggered when a block is updated")]
    public async Task<PollingEventResponse<DateMemory, List<BlockResponse>>> OnBlockUpdated(
        PollingEventRequest<DateMemory> pollingRequest)
    {
        var pollingStart = PollingEventResults.PollingStart;
        
        if (pollingRequest.Memory is null)
            return PollingEventResults.NoFlight<List<BlockResponse>>(pollingStart);
        
        RestRequest request = new SearchCriteriaBuilder()
            .WhereGreaterThan(("update_time", pollingRequest.Memory.LastInteractionDate))
            .ApplyTo(new RestRequest("V1/cmsBlock/search"));
        
        var response = await Client.Paginate<BlockEntity>(request);
        var responseList = response.Where(x => x.UpdateTime > x.CreationTime).ToList();
        if (responseList.Count == 0)
            return PollingEventResults.NoFlight<List<BlockResponse>>(pollingStart);
        
        var blocks = responseList.Select(x => new BlockResponse(x)).ToList();
        return PollingEventResults.Flight(blocks, pollingStart);
    }
}