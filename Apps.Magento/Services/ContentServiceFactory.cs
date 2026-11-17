using Apps.Magento.Constants;
using Apps.Magento.Services.Concrete;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Magento.Services;

public static class ContentServiceFactory
{
    public static IContentService Create(string contentType, InvocationContext invocationContext)
    {
        return contentType.Trim().ToLowerInvariant() switch
        {
            ContentTypeConstants.Block => new BlockService(invocationContext),
            ContentTypeConstants.Page => new PageService(invocationContext),
            ContentTypeConstants.Product => new ProductService(invocationContext),
            _ => throw new PluginMisconfigurationException(
                $"Content type '{contentType}' is not supported. Supported: {string.Join(", ", ContentTypeConstants.SupportedContentTypes)}")
        };
    }

    public static List<IContentService> CreateMultiple(IEnumerable<string> contentTypes, InvocationContext invocationContext)
    {
        return [..contentTypes.Select(contentType => Create(contentType, invocationContext))];
    }
}