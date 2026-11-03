using Apps.Magento.Extensions;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Entities.StoreViews;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Magento.DataSources;

public class StoreViewDataSource(InvocationContext invocationContext) : AppInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public virtual async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new RestRequest("V1/store/storeViews");
        var storeViews = await Client.ExecuteWithErrorHandling<List<StoreViewEntity>>(request);
        return storeViews
            .Where(x => x.Name.Matches(context.SearchString))
            .Select(x => new DataSourceItem(x.Code, x.Name))
            .ToList();
    }
}