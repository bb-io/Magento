using Apps.Magento.Constants;
using Apps.Magento.Extensions;
using Apps.Magento.Models.Entities.Blocks;
using Apps.Magento.Models.Requests.Content;
using Apps.Magento.Models.Responses.Content;
using Apps.Magento.Models.Utility.Content;
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
        var blocks = string.IsNullOrWhiteSpace(input.StoreView) || input.StoreView == "all"
            ? await SearchBlocks(input, storeId: null)
            : await SearchBlocksVisibleIn(input, input.StoreView);

        return blocks.Select(x => new ContentResponse(x.Id, ContentType, x.Title, x.CreationTime, x.UpdateTime)).ToList();
    }

    private async Task<IEnumerable<BlockEntity>> SearchBlocksVisibleIn(SearchContentRequest input, string storeCode)
    {
        var storeView = await Client.GetStoreView(storeCode);

        // Items visible in a store view = assigned to it + assigned to 'All Store Views' (ID = 0)
        // Magento store filter only matches the exact store so ask for both and merge
        var storeSpecificTask = SearchBlocks(input, storeView.Id);
        var allStoreViewsTask = SearchBlocks(input, "0");
        await Task.WhenAll(storeSpecificTask, allStoreViewsTask);
        
        // Store-specific copies come first, so they win over the 'All Store Views' item with the same identifier
        return storeSpecificTask.Result.Concat(allStoreViewsTask.Result).DistinctBy(x => x.Identifier);
    }

    private Task<IEnumerable<BlockEntity>> SearchBlocks(SearchContentRequest input, string? storeId)
    {
        var request = new SearchCriteriaBuilder()
            .WhereContains(("title", input.NameContains))
            .WhereLessThan(("creation_time", input.CreatedBefore))
            .WhereGreaterThan(("creation_time", input.CreatedAfter))
            .WhereLessThan(("update_time", input.UpdatedBefore))
            .WhereGreaterThan(("update_time", input.UpdatedAfter))
            .WhereEqual(("store_id", storeId))
            .ApplyTo(new RestRequest("V1/cmsBlock/search"));

        return Client.Paginate<BlockEntity>(request);
    }

    public async Task<DownloadedContent> DownloadContent(DownloadContentServiceRequest input)
    {
        string contentId = input.ContentId;
        var block = await Client.GetBlock(contentId);
        
        if (!string.IsNullOrWhiteSpace(input.StoreView) && input.StoreView != "all")
            block = await FindBlockInStoreView(block.Identifier, input.StoreView) ?? block;
        
        if (string.IsNullOrWhiteSpace(block.Content))
            throw new PluginMisconfigurationException("The content of this block is empty.");
        
        var storeConfig = await Client.GetStoreConfig(StoreViewHelper.ForRead(input.StoreView));
        var metadata = new ContentMetadata(
            ContentType,
            contentId,
            storeConfig.Language,
            new Ucid(ContentType, block.Identifier),
            block.Title);
        
        var htmlStream = HtmlHelper.ConvertToHtml(metadata, block.Content);
        return new(htmlStream, "text/html", block.Title.ToHtmlFilename(block.Identifier));
    }

    public async Task<UploadedContent> UploadContent(UploadContentServiceRequest input)
    {
        var source = await Client.GetBlock(input.ContentId);

        if (string.IsNullOrWhiteSpace(input.StoreView) || input.StoreView == "all")
        {
            source.Content = input.Html;
            var updated = await Client.UpdateBlock(source);
            return new(new Ucid(ContentType, updated.Identifier), updated.Title);
        }
        
        var target = await FindBlockInStoreView(source.Identifier, input.StoreView);
        var block = target ?? source;
        block.Content = input.Html;

        try
        {
            var saved = target is null
                ? await Client.CreateBlock(block, input.StoreView)
                : await Client.UpdateBlock(block, input.StoreView);

            return new(new Ucid(ContentType, saved.Identifier), saved.Title);
        }
        catch (PluginApplicationException ex) when (ex.Message.Contains("URL key for specified store already exists"))
        {
            throw new PluginMisconfigurationException(ErrorMessages.UrlKeyAlreadyExists(ContentType, source.Identifier, input.StoreView));
        }
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