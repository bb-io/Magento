using System.Globalization;
using Apps.Magento.Api;
using Apps.Magento.Constants;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Identifiers;
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

[ActionList]
public class PageActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : AppInvocable(invocationContext)
{
    [Action("Search pages", Description = "Retrieve all pages that match the specified criteria")]
    public async Task<PagesResponse> GetAllPagesAsync([ActionParameter] StoreViewOptionalIdentifier storeViewIdentifier, 
        [ActionParameter] FilterPageRequest filterRequest)
    {
        var queryString = BuildQueryString(filterRequest);
        var requestUrl = $"/rest/{storeViewIdentifier}/V1/cmsPage/search?searchCriteria{queryString}";
        var request = new ApiRequest(requestUrl, Method.Get, Creds);
        return await Client.ExecuteWithErrorHandling<PagesResponse>(request);
    }

    [Action("Get page", Description = "Get page by specified ID")]
    public async Task<PageResponse> GetPageByIdAsync([ActionParameter] StoreViewOptionalIdentifier storeViewIdentifier, 
        [ActionParameter] PageIdentifier identifier)
    {
        return await Client.ExecuteWithErrorHandling<PageResponse>(
            new ApiRequest($"/rest/{storeViewIdentifier}/V1/cmsPage/{identifier.PageId}", Method.Get, Creds));
    }
    
    [Action("Get page as HTML", Description = "Get page by specified ID as HTML")]
    public async Task<FileReference> GetPageByIdAsHtmlAsync([ActionParameter] StoreViewOptionalIdentifier storeViewIdentifier, 
        [ActionParameter] PageIdentifier identifier)
    {
        var page = await GetPageByIdAsync(storeViewIdentifier, identifier);
        var htmlStream = HtmlHelper.ConvertToHtml(ContentTypeConstants.Page, identifier.PageId, page.Content);
        return await fileManagementClient.UploadAsync(htmlStream, "text/html", $"{page.Identifier}.html");
    }

    [Action("Create page", Description = "Create a new page")]
    public async Task<PageResponse> CreatePageAsync([ActionParameter] StoreViewOptionalIdentifier storeViewIdentifier,
        [ActionParameter] CreatePageRequest pageRequest)
    {
        var request = new ApiRequest($"/rest/{storeViewIdentifier}/V1/cmsPage", Method.Post, Creds)
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
                    creation_time = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
                    sort_order = pageRequest.SortOrder,
                    active = true
                }
            });

        return await Client.ExecuteWithErrorHandling<PageResponse>(request);
    }

    [Action("Update page", Description = "Update page by specified ID")]
    public async Task<PageResponse> UpdatePageByIdAsync([ActionParameter] StoreViewOptionalIdentifier storeViewIdentifier,
        [ActionParameter] PageIdentifier identifier,
        [ActionParameter] UpdatePageRequest pageRequest)
    {
        ValidateRequestIfAllPropertiesAreNullThrowException(pageRequest);        
        
        var page = await GetPageByIdAsync(storeViewIdentifier, identifier);
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

        var request = new ApiRequest($"/rest/{storeViewIdentifier}/V1/cmsPage/{identifier.PageId}", Method.Put, Creds)
            .AddBody(new { page });
        return await Client.ExecuteWithErrorHandling<PageResponse>(request);
    }
    
    [Action("Update page from HTML", Description = "Update page from HTML document. Recommended to use with action: Get page as HTML")]
    public async Task<PageResponse> UpdatePageByIdFromHtmlAsync([ActionParameter] StoreViewOptionalIdentifier storeViewIdentifier, 
        [ActionParameter] UpdatePageFromHtmlRequest request)
    {
        var htmlStream = await fileManagementClient.DownloadAsync(request.File);
        var html = await new StreamReader(htmlStream).ReadToEndAsync();
        var htmlModel = HtmlHelper.ConvertFromHtml(html);
        
        var pageId = request.PageId ?? htmlModel.ResourceId ?? throw new ArgumentException("Couldn't find page ID in the HTML document. " +
            "Please specify it manually in the optional input.");
        return await UpdatePageByIdAsync(storeViewIdentifier, new PageIdentifier
        {
            PageId = pageId
        }, new UpdatePageRequest
        {
            Content = htmlModel.Content
        });
    }

    [Action("Delete page", Description = "Delete page by specified ID")]
    public async Task DeletePageByIdAsync([ActionParameter] StoreViewOptionalIdentifier storeViewIdentifier, 
        [ActionParameter] PageIdentifier identifier)
    {
        await Client.ExecuteWithErrorHandling(
            new ApiRequest($"/rest/{storeViewIdentifier}/V1/cmsPage/{identifier.PageId}", Method.Delete, Creds));
    }
}