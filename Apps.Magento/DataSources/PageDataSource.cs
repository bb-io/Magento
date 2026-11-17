using Apps.Magento.Invocables;
using Apps.Magento.Models.Entities.Pages;
using Apps.Magento.Utils;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Magento.DataSources;

public class PageDataSource(InvocationContext invocationContext) : AppInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        RestRequest request = new SearchCriteriaBuilder()
            .WhereContains(("title", context.SearchString))
            .ApplyTo(new RestRequest("V1/cmsPage/search"));
        
        var response = await Client.Paginate<PageEntity>(request, maxPages: 1);
        return response.Select(x => new DataSourceItem(x.Id, x.Title));
    }
}