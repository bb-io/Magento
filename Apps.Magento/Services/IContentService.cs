using Apps.Magento.Models.Requests.Content;
using Apps.Magento.Models.Responses.Content;
using Apps.Magento.Models.Utility.Content;
using Apps.Magento.Services.Models;

namespace Apps.Magento.Services;

public interface IContentService
{
    Task<List<ContentResponse>> SearchContent(SearchContentRequest input);
    Task<DownloadedContent> DownloadContent(DownloadContentServiceRequest input);
    Task<UploadedContent> UploadContent(UploadContentServiceRequest input);
    
    async Task<PolledContent> PollContent(PollContentServiceRequest input)
    {
        var request = new SearchContentRequest
        {
            NameContains = input.NameContains,
            UpdatedAfter = input.UpdatedAfter,
            StoreView = input.StoreView
        };

        var response = await SearchContent(request);
        return new(response, []);
    }
}