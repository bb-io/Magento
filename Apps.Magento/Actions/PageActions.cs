using Apps.Magento.Constants;
using Apps.Magento.Extensions;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Entities.Pages;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Identifiers.Optional;
using Apps.Magento.Models.Requests.Pages;
using Apps.Magento.Models.Responses.Pages;
using Apps.Magento.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
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
    
    [Action("Get page as HTML", Description = "Get page by specified ID as HTML")]
    public async Task<FileReference> GetPageByIdAsHtmlAsync([ActionParameter] StoreViewOptionalIdentifier storeViewIdentifier, 
        [ActionParameter] PageIdentifier identifier)
    {
        var page = await Client.GetPage(identifier.PageId);
        var htmlStream = HtmlHelper.ConvertToHtml(ContentTypeConstants.Page, identifier.PageId, page.Content);
        return await fileManagementClient.UploadAsync(htmlStream, "text/html", $"{page.Identifier}.html");
    }

    [Action("Create page", Description = "Create a new page")]
    public async Task<PageResponse> CreatePageAsync([ActionParameter] CreatePageRequest pageRequest)
    {
        var request = new RestRequest("V1/cmsPage", Method.Post)
            .AddBody(new
            {
                page = new
                {
                    identifier = pageRequest.Identifier,
                    title = pageRequest.Title,
                    page_layout = pageRequest.PageLayout,
                    meta_title = pageRequest.MetaTitle,
                    meta_keywords = pageRequest.MetaKeywords,
                    meta_description = pageRequest.MetaDescription,
                    content_heading = pageRequest.ContentHeading,
                    content = pageRequest.Content,
                    sort_order = pageRequest.SortOrder,
                    active = true
                }
            });

        return await Client.ExecuteWithErrorHandling<PageResponse>(request);
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