using Apps.Magento.Constants;
using Apps.Magento.Extensions;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Entities.Blocks;
using Apps.Magento.Models.Identifiers;
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
    
    [Action("Get block as HTML", Description = "Get block by specified ID as HTML")]
    public async Task<FileReference> GetBlockAsHtmlAsync([ActionParameter] BlockIdentifier identifier)
    {
        var block = await Client.GetBlock(identifier.BlockId);
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
        
        var request = new RestRequest("V1/cmsBlock", Method.Post).AddBody(body);
        var blockResponse = await Client.ExecuteWithErrorHandling<BlockResponse>(request);
        
        var createdBlock = await Client.GetBlock(blockResponse.Id);
        return new(createdBlock);
    }
     
    [Action("Update block", Description = "Update block with specified data")]
    public async Task<BlockResponse> UpdateBlockAsync([ActionParameter] BlockIdentifier identifier, 
        [ActionParameter] UpdateBlockRequest updateBlockRequest)
    {
        ValidateRequestIfAllPropertiesAreNullThrowException(updateBlockRequest);
        
        var block = await Client.GetBlock(identifier.BlockId);
        block.Identifier = updateBlockRequest.Identifier ?? block.Identifier;
        block.Title = updateBlockRequest.Title ?? block.Title;
        block.Content = updateBlockRequest.Content ?? block.Content;
        block.Active = updateBlockRequest.Active ?? block.Active;
        
        var request = new RestRequest($"V1/cmsBlock/{identifier.BlockId}", Method.Put).AddBody(new { block });
        var blockResponse = await Client.ExecuteWithErrorHandling<BlockResponse>(request);
        
        var updatedBlock = await Client.GetBlock(blockResponse.Id);
        return new(updatedBlock);
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
        var request = new RestRequest($"V1/cmsBlock/{identifier.BlockId}", Method.Delete);
        await Client.ExecuteWithErrorHandling(request);
    }
}