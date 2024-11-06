using Apps.Magento.Api;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Magento.DataSources;

public class StoreViewDataSource(InvocationContext invocationContext) : AppInvocable(invocationContext), IAsyncDataSourceHandler
{
    public virtual async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new ApiRequest("/rest/V1/store/storeViews", Method.Get, Creds);
        var storeViews = await Client.ExecuteWithErrorHandling<List<StoreViewDto>>(request);
        return storeViews
            .Where(x => context.SearchString == null || x.Name.Contains(context.SearchString))
            .ToDictionary(x => x.Code, x => x.Name);
    }
}