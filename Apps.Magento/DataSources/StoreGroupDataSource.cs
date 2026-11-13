using Apps.Magento.Extensions;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Entities.StoreGroups;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Magento.DataSources;

public class StoreGroupDataSource(InvocationContext context) : AppInvocable(context), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new RestRequest("V1/store/storeGroups");
        var result = await Client.ExecuteWithErrorHandling<List<StoreGroupEntity>>(request);
        return result
            .Where(x => x.Name.Matches(context.SearchString))
            .Select(x => new DataSourceItem(x.Id, x.Name))
            .ToList();
    }
}