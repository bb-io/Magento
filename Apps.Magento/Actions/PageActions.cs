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
    public async Task<PageResponse> UpdatePage(
        [ActionParameter] PageIdentifier identifier,
        [ActionParameter] UpdatePageRequest input)
    {
        RequestValidator.EnsureAnyFilled(input);
        
        var page = await Client.GetPage(identifier.PageId);
        page.Identifier = input.Identifier ?? page.Identifier;
        page.Title = input.Title ?? page.Title;
        page.MetaTitle = input.MetaTitle ?? page.MetaTitle;
        page.MetaKeywords = input.MetaKeywords ?? page.MetaKeywords;
        page.MetaDescription = input.MetaDescription ?? page.MetaDescription;
        page.ContentHeading = input.ContentHeading ?? page.ContentHeading;
        page.Active = input.Active ?? page.Active;

        var updatedPageEntity = await Client.UpdatePage(page);
        return new(updatedPageEntity);
    }
    
    [Action("Upload page", Description = "Upload page content")]
    public async Task UploadPage(
        [ActionParameter] PageOptionalIdentifier pageIdentifier,
        [ActionParameter] UploadPageRequest input)
    {
        var htmlModel = await fileManagementClient.DownloadHtml(input.File);
        string pageId = ContentResolver.ResolveContentId(pageIdentifier.PageId, htmlModel.ResourceId, "page ID");
        
        var page = await Client.GetPage(pageId);
        page.Content = htmlModel.Content;
        
        await Client.UpdatePage(page);
    }

    [Action("Delete page", Description = "Delete page by specified ID")]
    public Task DeletePage([ActionParameter] PageIdentifier identifier)
    {
        var request = new RestRequest($"V1/cmsPage/{identifier.PageId}", Method.Delete);
        return Client.ExecuteWithErrorHandling(request);
    }
}