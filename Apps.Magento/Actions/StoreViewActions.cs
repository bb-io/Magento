using Apps.Magento.Extensions;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Requests.StoreViews;
using Apps.Magento.Models.Responses.StoreViews;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Magento.Actions;

[ActionList("Store views")]
public class StoreViewActions(InvocationContext invocationContext) : AppInvocable(invocationContext)
{
    [Action("Search store views", Description = "Search store views by specific criteria")]
    public async Task<SearchStoreViewsResponse> SearchStoreViews([ActionParameter] SearchStoreViewsRequest input)
    {
        var storeViews = await Client.ListStoreViews();
        var result = storeViews
            .Where(x => x.Name.Matches(input.NameContains))
            .Select(x => new StoreViewResponse(x))
            .ToList();

        return new(result);
    }

    [Action("Get store view", Description = "Get store view by its code")]
    public async Task<StoreViewResponse> GetStoreView([ActionParameter] StoreViewIdentifier storeViewIdentifier)
    {
        var storeView = await Client.GetStoreView(storeViewIdentifier.StoreView);
        return new(storeView);
    }

    [Action("Create store view", Description = 
        "Create a new store view. Requires the Blackbird Store Provisioning extension to be installed on the Magento instance")]
    public async Task<StoreViewResponse> CreateStoreView([ActionParameter] CreateStoreViewRequest input)
    {
        var body = new
        {
            name = input.Name,
            code = input.Code,
            groupId = input.GroupId,
            locale = input.Locale,
            isActive = input.IsActive ?? true,
        };
        
        var request = new RestRequest("V1/blackbird/store-provisioning/store-view", Method.Post).AddBody(body);
        await Client.ExecuteWithErrorHandling(request);
        
        var createdStoreView = await Client.GetStoreView(input.Code.Trim().ToLowerInvariant());
        return new(createdStoreView);
    }
}