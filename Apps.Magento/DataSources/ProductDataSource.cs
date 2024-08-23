using Apps.Magento.Actions;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Requests.Products;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Magento.DataSources;

public class ProductDataSource(InvocationContext invocationContext) : AppInvocable(invocationContext), IAsyncDataSourceHandler
{
    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var actions = new ProductActions(InvocationContext, null!);
        var pages = await actions.GetAllProductsAsync(new StoreViewOptionalIdentifier(), new FilterProductRequest()
        {
            Title = context.SearchString
        });
        
        return pages.Items
            .ToDictionary(x => x.Sku, x => x.Name);
    }
}