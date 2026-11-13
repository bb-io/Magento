using Apps.Magento.Invocables;
using Apps.Magento.Models.Entities.Products;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Magento.DataSources;

public class ProductTypeDataSource(InvocationContext invocationContext) 
    : AppInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new RestRequest("V1/products/types");
        var storeViews = await Client.ExecuteWithErrorHandling<List<ProductTypeEntity>>(request);
        return storeViews
            .Where(x => context.SearchString == null || x.Name.Contains(context.SearchString))
            .Select(x => new DataSourceItem(x.Name, x.Label))
            .ToList();
    }
}