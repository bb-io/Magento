using Apps.Magento.Extensions;
using Apps.Magento.Invocables;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Magento.DataSources;

public class StoreViewDataSource(InvocationContext invocationContext) : AppInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public virtual async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var storeViews = await Client.ListStoreViews();
        return storeViews
            .Where(x => x.Name.Matches(context.SearchString))
            .Select(x => new DataSourceItem(x.Code, x.Name))
            .ToList();
    }
}