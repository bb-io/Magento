using Apps.Magento.Actions;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Responses.Pages;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Magento.DataSources;

public class PageDataSource(InvocationContext invocationContext) : AppInvocable(invocationContext), IAsyncDataSourceHandler
{
    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var actions = new PageActions(invocationContext);
        var pages = await actions.GetAllPagesAsync(new FilterPageRequest()
        {
            ConditionType = "like",
            Title = context.SearchString
        });
        
        return pages.Items
            .ToDictionary(x => x.Id, x => x.Title);
    }
}