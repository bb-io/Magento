using System.Globalization;
using System.Text;
using Apps.Magento.Api;
using Apps.Magento.Constants;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Requests;
using Apps.Magento.Models.Requests.Blocks;
using Apps.Magento.Models.Responses.Blocks;
using Apps.Magento.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;

namespace Apps.Magento.Actions;

[ActionList]
public class BlockActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : AppInvocable(invocationContext)
{
    [Action("Search blocks", Description = "Retrieve all blocks that match the specified criteria")]
    public async Task<BlocksResponse> GetAllBlocksAsync([ActionParameter] FilterBlockRequest filterRequest)
    {
        var queryString = BuildQueryString(filterRequest);
        var requestUrl = $"/rest/V1/cmsBlock/search?searchCriteria{queryString}";

        var request = new ApiRequest(requestUrl, Method.Get, Creds);
        return await Client.ExecuteWithErrorHandling<BlocksResponse>(request);
    }
    
    [Action("Get block", Description = "Get block by specified ID")]
    public async Task<BlockResponse> GetBlockAsync([ActionParameter] BlockIdentifier identifier)
    {
        var requestUrl = $"/rest/V1/cmsBlock/{identifier.BlockId}";
        var request = new ApiRequest(requestUrl, Method.Get, Creds);
        return await Client.ExecuteWithErrorHandling<BlockResponse>(request);
    }
    
    [Action("Get block as HTML", Description = "Get block by specified ID as HTML")]
    public async Task<FileReference> GetBlockAsHtmlAsync([ActionParameter] BlockIdentifier identifier)
    {
        var block = await GetBlockAsync(identifier);
        var htmlStream = HtmlHelper.ConvertToHtml(ContentTypeConstants.Block, identifier.BlockId, block.Content);
        return await fileManagementClient.UploadAsync(htmlStream, "text/html", $"{block.Identifier}.html");
    }
    
    [Action("Create block", Description = "Create block with specified data")]
    public async Task<BlockResponse> CreateBlockAsync([ActionParameter] CreateBlockRequest createBlockRequest)
    {
        var body = new
        {
            block = new
            {
                identifier = createBlockRequest.Identifier,
                title = createBlockRequest.Title,
                content = createBlockRequest.Content,
                active = true
            }
        };
        
        var request = new ApiRequest("/rest/V1/cmsBlock", Method.Post, Creds)
            .AddBody(body);
        var blockResponse = await Client.ExecuteWithErrorHandling<BlockResponse>(request);
        
        return await GetBlockAsync(new BlockIdentifier
        {
            BlockId = blockResponse.Id
        });
    }
     
    [Action("Update block", Description = "Update block with specified data")]
    public async Task<BlockResponse> UpdateBlockAsync([ActionParameter] BlockIdentifier identifier, 
        [ActionParameter] UpdateBlockRequest updateBlockRequest)
    {
        ValidateRequestIfAllPropertiesAreNullThrowException(updateBlockRequest);
        
        var block = await GetBlockAsync(identifier);
        block.Identifier = updateBlockRequest.Identifier ?? block.Identifier;
        block.Title = updateBlockRequest.Title ?? block.Title;
        block.Content = updateBlockRequest.Content ?? block.Content;
        block.Active = updateBlockRequest.Active ?? block.Active;
        
        var request = new ApiRequest($"/rest/V1/cmsBlock/{identifier.BlockId}", Method.Put, Creds)
            .AddBody(new { block });
        var blockResponse = await Client.ExecuteWithErrorHandling<BlockResponse>(request);
        return await GetBlockAsync(new() { BlockId = blockResponse.Id });
    }
    
    [Action("Update block from HTML", Description = "Update block with specified data from HTML")]
    public async Task<BlockResponse> UpdateBlockFromHtmlAsync( 
        [ActionParameter] UpdateBlockFromHtmlRequest request)
    {
        var htmlStream = await fileManagementClient.DownloadAsync(request.File);
        var html = await new StreamReader(htmlStream).ReadToEndAsync();
        var htmlModel = HtmlHelper.ConvertFromHtml(html);
        
        var blockId = request.BlockId ?? htmlModel.ResourceId ?? throw new ArgumentException("Couldn't find block ID in the HTML document. " +
            "Please specify it manually in the optional input.");
        return await UpdateBlockAsync(new BlockIdentifier()
        {
            BlockId = blockId
        }, new UpdateBlockRequest
        {
            Content = htmlModel.Content
        });
    }
    
    [Action("Delete block", Description = "Delete block by specified ID")]
    public async Task DeleteBlockAsync([ActionParameter] BlockIdentifier identifier)
    {
        var requestUrl = $"/rest/V1/cmsBlock/{identifier.BlockId}";
        var request = new ApiRequest(requestUrl, Method.Delete, Creds);
        await Client.ExecuteWithErrorHandling(request);
    }
    
    protected override string BuildQueryString(BaseFilterRequest filterRequest)
    {
        var queryString = new StringBuilder();
        var filterIndex = 0;

        if (!string.IsNullOrEmpty(filterRequest.Title))
        {
            queryString.Append($"[filterGroups][{filterIndex}][filters][0][field]={Uri.EscapeDataString("title")}");
            queryString.Append(
                $"&searchCriteria[filterGroups][{filterIndex}][filters][0][value]={Uri.EscapeDataString($"%{filterRequest.Title}%")}");
            queryString.Append(
                $"&searchCriteria[filterGroups][{filterIndex}][filters][0][conditionType]={Uri.EscapeDataString("like")}");

            filterIndex += 1;
        }
        
        if (filterRequest.CreatedAt.HasValue)
        {
            queryString.Append($"[filterGroups][0][filters][{filterIndex}][field]={Uri.EscapeDataString("creation_time")}");
            queryString.Append(
                $"&searchCriteria[filterGroups][0][filters][{filterIndex}][value]={Uri.EscapeDataString(filterRequest.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss"))}");
            queryString.Append(
                $"&searchCriteria[filterGroups][0][filters][{filterIndex}][conditionType]={Uri.EscapeDataString("gt")}");
            
            filterIndex += 1;
        }

        if (filterRequest.UpdatedAt.HasValue)
        {
            queryString.Append($"[filterGroups][0][filters][{filterIndex}][field]={Uri.EscapeDataString("update_time")}");
            queryString.Append(
                $"&searchCriteria[filterGroups][0][filters][{filterIndex}][value]={Uri.EscapeDataString(filterRequest.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss"))}");
            queryString.Append(
                $"&searchCriteria[filterGroups][0][filters][{filterIndex}][conditionType]={Uri.EscapeDataString("gt")}");
            
            filterIndex += 1;
        }

        return queryString.ToString();
    }
}