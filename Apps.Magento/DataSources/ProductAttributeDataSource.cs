using Apps.Magento.Invocables;
using Apps.Magento.Models.Entities.Products;
using Apps.Magento.Utils;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Magento.DataSources;

public class ProductAttributeDataSource(InvocationContext invocationContext) 
    : AppInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new SearchCriteriaBuilder()
            .WhereContains(("frontend_label", context.SearchString), ("attribute_code", context.SearchString))
            .ApplyTo(new RestRequest("V1/products/attributes"));
        
        var attributes = await Client.Paginate<ProductAttributeEntity>(request, maxPages: 1);
        return attributes.Select(x => new DataSourceItem(x.AttributeCode, x.DefaultFrontendLabel)).ToList();
    }
}