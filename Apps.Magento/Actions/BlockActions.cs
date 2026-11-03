using Apps.Magento.Constants;
using Apps.Magento.Extensions;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Entities.Blocks;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Identifiers.Optional;
using Apps.Magento.Models.Requests.Blocks;
using Apps.Magento.Models.Responses;
using Apps.Magento.Models.Responses.Blocks;
using Apps.Magento.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;

namespace Apps.Magento.Actions;

[ActionList("Blocks")]
public class BlockActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
    : AppInvocable(invocationContext)
{
    [Action("Search blocks", Description = "Search blocks by specific criteria")]
    public async Task<SearchBlocksResponse> SearchBlocks([ActionParameter] SearchBlocksRequest input)
    {
        RestRequest request = new SearchCriteriaBuilder()
            .WhereContains(("title", input.TitleContains))
            .WhereLessThan(("creation_time", input.CreatedBefore))
            .WhereGreaterThan(("creation_time", input.CreatedAfter))
            .WhereLessThan(("update_time", input.UpdatedBefore))
            .WhereGreaterThan(("update_time", input.UpdatedAfter))
            .ApplyTo(new RestRequest("V1/cmsBlock/search"));
        
        var response = await Client.Paginate<BlockEntity>(request);
        var blocks = response.Select(x => new BlockResponse(x)).ToList();
        return new(blocks);
    }
    
    [Action("Get block", Description = "Get block by specified ID")]
    public async Task<BlockResponse> GetBlock([ActionParameter] BlockIdentifier identifier)
    {
        var block = await Client.GetBlock(identifier.BlockId);
        return new(block);
    }
    
    [Action("Download block", Description = "Download block content")]
    public async Task<FileResponse> DownloadBlock([ActionParameter] BlockIdentifier identifier)
    {
        var block = await Client.GetBlock(identifier.BlockId);

        if (string.IsNullOrWhiteSpace(block.Content))
            throw new PluginMisconfigurationException("The content of this block is empty.");
        
        var htmlStream = HtmlHelper.ConvertToHtml(ContentTypeConstants.Block, identifier.BlockId, block.Content);
        var file = await fileManagementClient.UploadAsync(htmlStream, "text/html", $"{block.Identifier}.html");
        return new(file);
    }
    
    [Action("Create block", Description = "Create a new block")]
    public async Task<BlockResponse> CreateBlock(
        [ActionParameter] StoreViewWithAllOptionalIdentifier viewIdentifier,
        [ActionParameter] CreateBlockRequest input)
    {
        var content = input.File is null ? null : (await fileManagementClient.DownloadHtml(input.File)).Content;

        var body = new
        {
            block = new
            {
                identifier = input.Identifier,
                title = input.Title,
                content,
                active = input.Active ?? true
            }
        };
        
        var request = new RestRequest($"{viewIdentifier}/V1/cmsBlock", Method.Post).AddBody(body);
        var blockResponse = await Client.ExecuteWithErrorHandling<BlockEntity>(request);
        
        var createdBlock = await Client.GetBlock(blockResponse.Id);
        return new(createdBlock);
    }
     
    [Action("Update block", Description = "Update block with specified data")]
    public async Task<BlockResponse> UpdateBlock(
        [ActionParameter] BlockIdentifier identifier, 
        [ActionParameter] UpdateBlockRequest input)
    {
        RequestValidator.EnsureAnyFilled(input);
        
        var block = await Client.GetBlock(identifier.BlockId);
        block.Identifier = input.Identifier ?? block.Identifier;
        block.Title = input.Title ?? block.Title;
        block.Active = input.Active ?? block.Active;
        
        var updatedBlockEntity = await Client.UpdateBlock(block);
        return new(updatedBlockEntity);
    }
    
    [Action("Upload block", Description = "Upload content of a specific block")]
    public async Task UploadBlock(
        [ActionParameter] UploadBlockRequest request,
        [ActionParameter] BlockOptionalIdentifier blockIdentifier)
    {
        var htmlModel = await fileManagementClient.DownloadHtml(request.File);
        string blockId = ContentResolver.ResolveContentId(blockIdentifier.BlockId, htmlModel.ResourceId, "block ID");
        
        var block = await Client.GetBlock(blockId);
        block.Content = htmlModel.Content;
        
        await Client.UpdateBlock(block);
    }
    
    [Action("Delete block", Description = "Delete block by specified ID")]
    public Task DeleteBlock([ActionParameter] BlockIdentifier identifier)
    {
        var request = new RestRequest($"V1/cmsBlock/{identifier.BlockId}", Method.Delete);
        return Client.ExecuteWithErrorHandling(request);
    }
}