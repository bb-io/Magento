using Apps.Magento.Constants;
using Apps.Magento.Models.Requests.Content;
using Apps.Magento.Models.Responses.Content;
using Apps.Magento.Models.Utility.Content;
using Apps.Magento.Services;
using Apps.Magento.Services.Models;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Magento.Utils;

public static class ContentHelper
{
    public static async Task<List<ContentResponse>> Search(
        List<string>? inputContentTypes,
        SearchContentRequest input,
        InvocationContext invocationContext)
    {
        var contentTypes = inputContentTypes ?? ContentTypeConstants.SupportedContentTypes;
        var services = ContentServiceFactory.CreateMultiple(contentTypes.Distinct(), invocationContext);

        var searchTasks = services.Select(x => x.SearchContent(input));
        var searchResults = await Task.WhenAll(searchTasks);
        return searchResults.SelectMany(x => x).ToList();
    }
    
    public static async Task<PolledContent> Poll(
        List<string>? inputContentTypes,
        PollContentServiceRequest input,
        InvocationContext invocationContext)
    {
        var contentTypes = inputContentTypes ?? ContentTypeConstants.SupportedContentTypes;
        var services = ContentServiceFactory.CreateMultiple(contentTypes.Distinct(), invocationContext);

        var results = await Task.WhenAll(services.Select(x => x.PollContent(input)));
        return new(
            results.SelectMany(x => x.Changed).ToList(),
            results.SelectMany(x => x.Hashes).ToDictionary(x => x.Key, x => x.Value));
    }
}