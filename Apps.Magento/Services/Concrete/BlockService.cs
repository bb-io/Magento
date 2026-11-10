using Apps.Magento.Constants;
using Apps.Magento.Extensions;
using Apps.Magento.Models.Entities.Blocks;
using Apps.Magento.Models.Requests.Content;
using Apps.Magento.Models.Responses.Content;
using Apps.Magento.Models.Utility.Files;
using Apps.Magento.Services.Models;
using Apps.Magento.Utils;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Magento.Services.Concrete;

public class BlockService(InvocationContext invocationContext) : BaseContentService(invocationContext), IContentService
{
    protected override string ContentType => ContentTypeConstants.Block;
    
    public async Task<List<ContentResponse>> SearchContent(SearchContentRequest input)
    {
        RestRequest request = new SearchCriteriaBuilder()
            .WhereContains(("title", input.NameContains))
            .WhereLessThan(("creation_time", input.CreatedBefore))
            .WhereGreaterThan(("creation_time", input.CreatedAfter))
            .WhereLessThan(("update_time", input.UpdatedBefore))
            .WhereGreaterThan(("update_time", input.UpdatedAfter))
            .ApplyTo(new RestRequest("V1/cmsBlock/search"));
        
        var response = await Client.Paginate<BlockEntity>(request);
        return response.Select(x => new ContentResponse(x.Id, ContentType, x.Title, x.CreationTime, x.UpdateTime)).ToList();
    }

    public async Task<DownloadedContent> DownloadContent(DownloadContentServiceRequest input)
    {
        var block = await Client.GetBlock(input.ContentId);
        if (string.IsNullOrWhiteSpace(block.Content))
            throw new PluginMisconfigurationException("The content of this block is empty.");
        
        var htmlStream = HtmlHelper.ConvertToHtml(ContentType, input.ContentId, block.Content);
        return new(htmlStream, "text/html", block.Identifier.ToHtmlFilename());
    }

    public async Task<UploadedContent> UploadContent(UploadContentServiceRequest input)
    {
        var source = await Client.GetBlock(input.ContentId);

        if (string.IsNullOrWhiteSpace(input.StoreView) || input.StoreView == "all")
        {
            source.Content = input.Html;
            var updated = await Client.UpdateBlock(source);
            return new(updated.Id, updated.Title);
        }

        var target = await FindBlockInStoreView(source.Identifier, input.StoreView);

        if (target?.Id == source.Id)
        {
            throw new PluginMisconfigurationException(
                $"Block '{source.Identifier}' is assigned to all store views, so a translated copy can't be created. " +
                "Assign the source block to a specific store view first.");
        }

        if (target is null)
        {
            source.Content = input.Html;
            var created = await Client.CreateBlock(source, input.StoreView);
            return new(created.Id, created.Title);
        }

        target.Content = input.Html;
        var updatedTarget = await Client.UpdateBlock(target, input.StoreView);
        return new(updatedTarget.Id, updatedTarget.Title);
    }

    private async Task<BlockEntity?> FindBlockInStoreView(string identifier, string storeCode)
    {
        var storeView = await Client.GetStoreView(storeCode);

        var request = new SearchCriteriaBuilder()
            .WhereEqual(("identifier", identifier))
            .WhereEqual(("store_id", storeView.Id))
            .ApplyTo(new RestRequest("V1/cmsBlock/search"));

        var blocks = await Client.Paginate<BlockEntity>(request, maxPages: 1);
        return blocks.FirstOrDefault();
    }
}