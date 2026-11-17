using Apps.Magento.Extensions;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Magento.DataSources;

public class StoreViewWithAllDataSource(InvocationContext invocationContext) : StoreViewDataSource(invocationContext)
{
    public override async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var storeViews = await base.GetDataAsync(context, cancellationToken);
        return "All store views".Matches(context.SearchString)
            ? storeViews.Prepend(new DataSourceItem("all", "All store views"))
            : storeViews;
    }
}