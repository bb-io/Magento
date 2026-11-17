using Apps.Magento.Invocables;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Identifiers.Optional;
using Apps.Magento.Models.Responses.Content;
using Apps.Magento.Polling.Models;
using Apps.Magento.Polling.Models.Requests;
using Apps.Magento.Services.Models;
using Apps.Magento.Utils;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;

namespace Apps.Magento.Polling;

[PollingEventList("Content")]
public class ContentPollingList(InvocationContext invocationContext) : AppInvocable(invocationContext)
{
    [BlueprintEventDefinition(BlueprintEvent.ContentCreatedOrUpdated)]
    [MultipleEvents, PollingEvent("On content created or updated", Description = "Triggered when a content item is created or updated")]
    public async Task<PollingEventResponse<ContentPollingMemory, List<ContentResponse>>> OnContentCreatedOrUpdated(
        PollingEventRequest<ContentPollingMemory> pollingRequest,
        [PollingEventParameter] ContentTypesOptionalIdentifier contentTypesIdentifier,
        [PollingEventParameter] OnContentCreatedOrUpdatedRequest input,
        [PollingEventParameter] StoreViewIdentifier storeViewIdentifier)
    {
        var pollingStart = PollingEventResults.PollingStart;
        var memory = pollingRequest.Memory;

        if (memory is null)
        {
            var updatedMemory = new ContentPollingMemory { LastInteractionDate = pollingStart };
            return PollingEventResults.NoFlight<ContentPollingMemory, List<ContentResponse>>(updatedMemory);
        }

        var pollRequest = new PollContentServiceRequest(
            memory.LastInteractionDate,
            input.NameContains,
            storeViewIdentifier.StoreView,
            memory.ContentHashes);

        var polled = await ContentHelper.Poll(contentTypesIdentifier.ContentTypes, pollRequest, InvocationContext);
        
        foreach (var (key, hash) in polled.Hashes)
            memory.ContentHashes[key] = hash;
        
        var newMemory = new ContentPollingMemory { LastInteractionDate = pollingStart, ContentHashes = memory.ContentHashes };
        
        return polled.Changed.Count == 0
            ? new() { FlyBird = false, Memory = newMemory }
            : new() { FlyBird = true, Result = polled.Changed, Memory = newMemory };
    }
}