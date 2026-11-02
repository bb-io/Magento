using System.Text;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Entities.Blocks;
using Apps.Magento.Models.Requests;
using Apps.Magento.Models.Responses.Blocks;
using Apps.Magento.Polling.Models;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;

namespace Apps.Magento.Polling;

[PollingEventList]
public class BlockPollingList(InvocationContext invocationContext) : AppInvocable(invocationContext)
{
    [PollingEvent("On blocks created", "Triggered when new blocks are created")]
    public async Task<PollingEventResponse<DateMemory, List<BlockResponse>>> OnBlocksCreated(
        PollingEventRequest<DateMemory> request)
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
        
        var blocks = await GetBlocks(new BaseFilterRequest { CreatedAt = request.Memory.LastInteractionDate });
        return new()
        {
            FlyBird = blocks.Any(),
            Result = blocks.Select(x => new BlockResponse(x)).ToList(),
            Memory = new()
            {
                LastInteractionDate = DateTime.UtcNow
            }
        };
    }
    
    [PollingEvent("On blocks updated", "Triggered when blocks are updated")]
    public async Task<PollingEventResponse<DateMemory, List<BlockResponse>>> OnBlocksUpdated(
        PollingEventRequest<DateMemory> request)
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
        
        var blocks = await GetBlocks(new BaseFilterRequest { UpdatedAt = request.Memory.LastInteractionDate });
        blocks = blocks.Where(x => x.CreationTime != x.UpdateTime).ToList();

        return new()
        {
            FlyBird = blocks.Any(),
            Result = blocks.Select(x => new BlockResponse(x)).ToList(),
            Memory = new()
            {
                LastInteractionDate = DateTime.UtcNow
            }
        };
    }
    
    private async Task<List<BlockEntity>> GetBlocks(BaseFilterRequest filter)
    {
        var queryString = BuildQueryString(filter);
        var request = new RestRequest($"V1/cmsBlock/search?searchCriteria{queryString}");
        var response = await Client.Paginate<BlockEntity>(request);
        return response.ToList();
    }
    
    protected string BuildQueryString(BaseFilterRequest filterRequest)
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