using Apps.Magento.Actions;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Requests.Blocks;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Magento.DataSources;

public class BlockDataSource(InvocationContext invocationContext) : AppInvocable(invocationContext), IAsyncDataSourceHandler
{
    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var actions = new BlockActions(invocationContext, null!);
        var pages = await actions.GetAllBlocksAsync(new StoreViewOptionalIdentifier(), new FilterBlockRequest
        {
            ConditionType = "like",
            Title = $"%{context.SearchString}%"
        });
        
        return pages.Items
            .ToDictionary(x => x.Id, x => x.Title);
    }
}