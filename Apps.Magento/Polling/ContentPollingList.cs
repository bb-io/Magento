using Apps.Magento.Invocables;
using Apps.Magento.Models.Identifiers.Optional;
using Apps.Magento.Models.Requests.Content;
using Apps.Magento.Models.Responses.Content;
using Apps.Magento.Polling.Models;
using Apps.Magento.Polling.Models.Requests;
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
    public async Task<PollingEventResponse<DateMemory, List<ContentResponse>>> OnContentCreatedOrUpdated(
        PollingEventRequest<DateMemory> pollingRequest,
        [PollingEventParameter] ContentTypesOptionalIdentifier contentTypesIdentifier,
        [PollingEventParameter] OnContentCreatedOrUpdatedRequest input)
    {
        var pollingStart = PollingEventResults.PollingStart;

        if (pollingRequest.Memory is null)
            return PollingEventResults.NoFlight<List<ContentResponse>>(pollingStart);
        
        var searchContentRequest = new SearchContentRequest
        {
            NameContains = input.NameContains,
            UpdatedAfter = pollingRequest.Memory.LastInteractionDate,
        };
        var result = await ContentHelper.Search(contentTypesIdentifier.ContentTypes, searchContentRequest, InvocationContext);
        
        return result.Count == 0 
            ? PollingEventResults.NoFlight<List<ContentResponse>>(pollingStart) 
            : PollingEventResults.Flight(result, pollingStart);
    }
}