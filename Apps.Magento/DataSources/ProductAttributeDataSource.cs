using Apps.Magento.Api;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Magento.DataSources;

public class ProductAttributeDataSource(InvocationContext invocationContext) : AppInvocable(invocationContext), IAsyncDataSourceHandler
{
    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new ApiRequest("/rest/default/V1/products/attributes?searchCriteria", Method.Get, Creds);
        var productAttributes = await Client.ExecuteWithErrorHandling<ProductAttributesDto>(request);
        return productAttributes.Items
            .Where(x => context.SearchString == null || x.DefaultFrontendLabel.Contains(context.SearchString))
            .ToDictionary(x => x.AttributeCode, x => x.DefaultFrontendLabel);
    }
}