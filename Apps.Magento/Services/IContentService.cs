using Apps.Magento.Models.Requests.Content;
using Apps.Magento.Models.Responses.Content;
using Apps.Magento.Models.Utility.Files;
using Apps.Magento.Services.Models;

namespace Apps.Magento.Services;

public interface IContentService
{
    Task<List<ContentResponse>> SearchContent(SearchContentRequest input);
    Task<DownloadedContent> DownloadContent(DownloadContentServiceRequest input);
    Task<UploadedContent> UploadContent(UploadContentServiceRequest input);
}