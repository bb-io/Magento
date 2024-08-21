using Apps.Magento.Api;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Magento.DataSources;

public class ProductTypeDataSource(InvocationContext invocationContext) : AppInvocable(invocationContext), IAsyncDataSourceHandler
{
    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new ApiRequest("/rest/default/V1/products/types", Method.Get, Creds);
        var storeViews = await Client.ExecuteWithErrorHandling<List<ProductTypeDto>>(request);
        return storeViews
            .Where(x => context.SearchString == null || x.Name.Contains(context.SearchString))
            .ToDictionary(x => x.Name, x => x.Label);
    }
}