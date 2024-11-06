using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Magento.DataSources;

public class StoreViewWithAllDataSource(InvocationContext invocationContext) : StoreViewDataSource(invocationContext)
{
    public override async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var result = await base.GetDataAsync(context, cancellationToken);
        
        if (result.All(x => x.Key != "all"))
        {
            result.Add("all", "All");
        }
        
        return result;
    }
}