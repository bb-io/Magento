using Apps.Magento.Actions;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Requests.Pages;
using Apps.Magento.Models.Responses.Pages;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Magento.DataSources;

public class PageDataSource(InvocationContext invocationContext) : AppInvocable(invocationContext), IAsyncDataSourceHandler
{
    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var actions = new PageActions(InvocationContext, null!);
        var pages = await actions.GetAllPagesAsync(new StoreViewOptionalIdentifier(), new FilterPageRequest()
        {
            Title = context.SearchString
        });
        
        return pages.Items
            .ToDictionary(x => x.Id, x => GetPageName(x, pages.Items));
    }
    
    private string GetPageName(PageResponse pageResponse, List<PageResponse> pages)
    {
        var pageWithSameName = pages.FirstOrDefault(x => x.Title == pageResponse.Title && x.Id != pageResponse.Id);
        if (pageWithSameName == null)
        {
            return pageResponse.Title;
        }
        
        return $"{pageResponse.Title} ({pageResponse.Identifier})";
    }
}