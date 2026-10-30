using Apps.Magento.Invocables;
using Apps.Magento.Models.Entities.Products;
using Apps.Magento.Models.Identifiers.Optional;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Magento.DataSources;

public class ProductDataSource(
    InvocationContext invocationContext,
    [ActionParameter] StoreViewOptionalIdentifier viewIdentifier) : AppInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new RestRequest($"{viewIdentifier}/V1/products");

        if (!string.IsNullOrWhiteSpace(context.SearchString))
        {
            const string filter = "searchCriteria[filterGroups][0][filters][0]";
            request.AddQueryParameter($"{filter}[field]", "name")
                .AddQueryParameter($"{filter}[value]", $"%{context.SearchString}%")
                .AddQueryParameter($"{filter}[conditionType]", "like");
        }

        var products = await Client.Paginate<ProductEntity>(request, maxPages: 1);
        return products.Select(x => new DataSourceItem(x.Sku, x.Name));
    }
}