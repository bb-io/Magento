using Apps.Magento.Constants;
using Apps.Magento.Extensions;
using Apps.Magento.Models.Entities.Pages;
using Apps.Magento.Models.Requests.Content;
using Apps.Magento.Models.Responses.Content;
using Apps.Magento.Models.Utility.Files;
using Apps.Magento.Services.Models;
using Apps.Magento.Utils;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Magento.Services.Concrete;

public class PageService(InvocationContext invocationContext) : BaseContentService(invocationContext), IContentService
{
    protected override string ContentType => ContentTypeConstants.Page;
    
    public async Task<List<ContentResponse>> SearchContent(SearchContentRequest input)
    {
        RestRequest request = new SearchCriteriaBuilder()
            .WhereContains(("title", input.NameContains))
            .WhereLessThan(("creation_time", input.CreatedBefore))
            .WhereGreaterThan(("creation_time", input.CreatedAfter))
            .WhereLessThan(("update_time", input.UpdatedBefore))
            .WhereGreaterThan(("update_time", input.UpdatedAfter))
            .ApplyTo(new RestRequest("V1/cmsPage/search"));
        
        var response = await Client.Paginate<PageEntity>(request);
        return response.Select(x => new ContentResponse(x.Id, ContentType, x.Title, x.CreationTime, x.UpdateTime)).ToList();
    }

    public async Task<DownloadedContent> DownloadContent(DownloadContentServiceRequest input)
    {
        var page = await Client.GetPage(input.ContentId);
        if (string.IsNullOrWhiteSpace(page.Content))
            throw new PluginMisconfigurationException("The content of this page is empty.");
        
        var htmlStream = HtmlHelper.ConvertToHtml(ContentType, input.ContentId, page.Content);
        return new(htmlStream, "text/html", page.Identifier.ToHtmlFilename());
    }

    public async Task<UploadedContent> UploadContent(UploadContentServiceRequest input)
    {
        var source = await Client.GetPage(input.ContentId);

        if (string.IsNullOrWhiteSpace(input.StoreView) || input.StoreView == "all")
        {
            source.Content = input.Html;
            var updated = await Client.UpdatePage(source);
            return new(updated.Id, updated.Title);
        }

        var target = await FindPageInStoreView(source.Identifier, input.StoreView);
        var page = target ?? source;
        page.Content = input.Html;

        try
        {
            var saved = target is null
                ? await Client.CreatePage(page, input.StoreView)
                : await Client.UpdatePage(page, input.StoreView);

            return new(saved.Id, saved.Title);
        }
        catch (PluginApplicationException ex) when (ex.Message.Contains("URL key for specified store already exists"))
        {
            throw new PluginMisconfigurationException(ErrorMessages.UrlKeyAlreadyExists(ContentType, source.Identifier, input.StoreView));
        }
    }

    private async Task<PageEntity?> FindPageInStoreView(string identifier, string storeCode)
    {
        var storeView = await Client.GetStoreView(storeCode);

        var request = new SearchCriteriaBuilder()
            .WhereEqual(("identifier", identifier))
            .WhereEqual(("store_id", storeView.Id))
            .ApplyTo(new RestRequest("V1/cmsPage/search"));

        var pages = await Client.Paginate<PageEntity>(request, maxPages: 1);
        return pages.FirstOrDefault();
    }
}