using Apps.Magento.Constants;
using Apps.Magento.Extensions;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Entities.Pages;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Identifiers.Optional;
using Apps.Magento.Models.Requests.Pages;
using Apps.Magento.Models.Responses;
using Apps.Magento.Models.Responses.Pages;
using Apps.Magento.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;

namespace Apps.Magento.Actions;

[ActionList("Pages")]
public class PageActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : AppInvocable(invocationContext)
{
    [Action("Search pages", Description = "Search pages by specific criteria")]
    public async Task<SearchPagesResponse> SearchPages([ActionParameter] SearchPagesRequest input)
    {
        RestRequest request = new SearchCriteriaBuilder()
            .WhereContains(("title", input.TitleContains))
            .WhereLessThan(("creation_time", input.CreatedBefore))
            .WhereGreaterThan(("creation_time", input.CreatedAfter))
            .WhereLessThan(("update_time", input.UpdatedBefore))
            .WhereGreaterThan(("update_time", input.UpdatedAfter))
            .ApplyTo(new RestRequest("V1/cmsPage/search"));
        
        var response = await Client.Paginate<PageEntity>(request);
        var pages = response.Select(x => new PageResponse(x)).ToList();
        return new(pages);
    }

    [Action("Get page", Description = "Get page by specified ID")]
    public async Task<PageResponse> GetPage([ActionParameter] PageIdentifier identifier)
    {
        var page = await Client.GetPage(identifier.PageId);
        return new(page);
    }
    
    [Action("Download page", Description = "Download page content")]
    public async Task<FileResponse> DownloadPage([ActionParameter] PageIdentifier identifier)
    {
        var page = await Client.GetPage(identifier.PageId);
        
        if (string.IsNullOrWhiteSpace(page.Content))
            throw new PluginMisconfigurationException("The content of this page is empty.");
        
        var htmlStream = HtmlHelper.ConvertToHtml(ContentTypeConstants.Page, identifier.PageId, page.Content);
        var file = await fileManagementClient.UploadAsync(htmlStream, "text/html", $"{page.Identifier.Replace('/', '_')}.html");
        return new(file);
    }

    [Action("Create page", Description = "Create a new page")]
    public async Task<PageResponse> CreatePage(
        [ActionParameter] StoreViewWithAllOptionalIdentifier viewIdentifier,
        [ActionParameter] CreatePageRequest input)
    {
        var content = input.File is null ? null : (await fileManagementClient.DownloadHtml(input.File)).Content;
        var body = new
        {
            page = new
            {
                identifier = input.Identifier,
                title = input.Title,
                meta_title = input.MetaTitle,
                meta_keywords = input.MetaKeywords,
                meta_description = input.MetaDescription,
                content_heading = input.ContentHeading,
                content,
                active = input.Active ?? true,
            }
        };
        
        string storeCode = viewIdentifier.StoreView ?? "all";
        var request = new RestRequest($"{storeCode}/V1/cmsPage", Method.Post).WithJsonBody(body, JsonConfig.JsonSettings);
        var response = await Client.ExecuteWithErrorHandling<PageEntity>(request);

        var createdPage = await Client.GetPage(response.Id);    // Refetch so that timestamps populate
        return new(createdPage);
    }

    [Action("Update page", Description = "Update page by specified ID")]
    public async Task<PageResponse> UpdatePageByIdAsync([ActionParameter] PageIdentifier identifier,
        [ActionParameter] UpdatePageRequest pageRequest)
    {
        ValidateRequestIfAllPropertiesAreNullThrowException(pageRequest);        
        
        var page = await Client.GetPage(identifier.PageId);
        page.Identifier = pageRequest.Identifier ?? page.Identifier;
        page.Title = pageRequest.Title ?? page.Title;
        page.PageLayout = pageRequest.PageLayout ?? page.PageLayout;
        page.MetaTitle = pageRequest.MetaTitle ?? page.MetaTitle;
        page.MetaKeywords = pageRequest.MetaKeywords ?? page.MetaKeywords;
        page.MetaDescription = pageRequest.MetaDescription ?? page.MetaDescription;
        page.ContentHeading = pageRequest.ContentHeading ?? page.ContentHeading;
        page.Content = pageRequest.Content ?? page.Content;
        page.SortOrder = pageRequest.SortOrder ?? page.SortOrder;
        page.Active = pageRequest.Active ?? page.Active;
        
        var request = new RestRequest($"V1/cmsPage/{identifier.PageId}", Method.Put)
            .AddBody(new { page = new
            {
                id = page.Id,
                identifier = page.Identifier,
                title = page.Title,
                page_layout = page.PageLayout,
                meta_title = page.MetaTitle,
                meta_keywords = page.MetaKeywords,
                meta_description = page.MetaDescription,
                content_heading = page.ContentHeading,
                content = page.Content,
                sort_order = page.SortOrder,
                active = page.Active
            } });
        
        await Client.ExecuteWithErrorHandling<PageResponse>(request);
        
        var updatedPage = await Client.GetPage(identifier.PageId);
        return new(updatedPage);
    }
    
    [Action("Update page from HTML", Description = "Update page from HTML document. Recommended to use with action: Get page as HTML")]
    public async Task<PageResponse> UpdatePageByIdFromHtmlAsync([ActionParameter] UpdatePageFromHtmlRequest request)
    {
        var htmlStream = await fileManagementClient.DownloadAsync(request.File);
        var html = await new StreamReader(htmlStream).ReadToEndAsync();
        var htmlModel = HtmlHelper.ConvertFromHtml(html);
        
        var pageId = request.PageId ?? htmlModel.ResourceId ?? throw new ArgumentException("Couldn't find page ID in the HTML document. " +
            "Please specify it manually in the optional input.");
        return await UpdatePageByIdAsync(new PageIdentifier
        {
            PageId = pageId
        }, new UpdatePageRequest
        {
            Content = htmlModel.Content
        });
    }

    [Action("Delete page", Description = "Delete page by specified ID")]
    public Task DeletePageByIdAsync([ActionParameter] PageIdentifier identifier)
    {
        return Client.ExecuteWithErrorHandling(new RestRequest($"V1/cmsPage/{identifier.PageId}", Method.Delete));
    }
}