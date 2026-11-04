using Apps.Magento.Models.Requests.Content;
using Apps.Magento.Models.Responses.Content;

namespace Apps.Magento.Services;

public interface IContentService
{
    Task<List<ContentResponse>> SearchContent(SearchContentRequest input);
}