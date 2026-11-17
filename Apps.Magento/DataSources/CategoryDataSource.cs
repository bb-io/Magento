using Apps.Magento.Invocables;
using Apps.Magento.Models.Entities.Categories;
using Apps.Magento.Utils;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Magento.DataSources;

public class CategoryDataSource(InvocationContext invocationContext) : AppInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        RestRequest request = new SearchCriteriaBuilder()
            .WhereContains(("name", context.SearchString))
            .ApplyTo(new RestRequest("V1/categories/list"));
        
        var response = await Client.Paginate<CategoryEntity>(request, maxPages: 1);
        return response.Select(x => new DataSourceItem(x.Id, x.Name)).ToList();
    }
}