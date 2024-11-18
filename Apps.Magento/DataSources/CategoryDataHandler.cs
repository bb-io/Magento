using Apps.Magento.Actions;
using Apps.Magento.Invocables;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Magento.DataSources;

public class CategoryDataHandler(InvocationContext invocationContext)
    : AppInvocable(invocationContext), IAsyncDataSourceHandler
{
    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var actions = new CategoryActions(InvocationContext);
        var pages = await actions.GetAllCategoriesAsync();

        return pages.Items
            .ToDictionary(x => x.Id, x => x.Name);
    }
}