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

    public async Task<DownloadedFile> DownloadContent(DownloadContentServiceRequest input)
    {
        var block = await Client.GetBlock(input.ContentId);
        if (string.IsNullOrWhiteSpace(block.Content))
            throw new PluginMisconfigurationException("The content of this block is empty.");
        
        var htmlStream = HtmlHelper.ConvertToHtml(ContentType, input.ContentId, block.Content);
        return new(htmlStream, "text/html", block.Identifier.ToHtmlFilename());
    }
}