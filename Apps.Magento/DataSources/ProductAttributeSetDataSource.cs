using Apps.Magento.Invocables;
using Apps.Magento.Models.Entities.Attributes;
using Apps.Magento.Utils;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Magento.DataSources;

public class ProductAttributeSetDataSource(InvocationContext invocationContext) 
    : AppInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new SearchCriteriaBuilder()
            .WhereContains(("attribute_set_name", context.SearchString))
            .ApplyTo(new RestRequest("V1/products/attribute-sets/sets/list"));

        var attributeSets = await Client.Paginate<AttributeSetEntity>(request, maxPages: 1);
        return attributeSets.Select(x => new DataSourceItem(x.AttributeSetId.ToString(), x.AttributeSetName));
    }
}