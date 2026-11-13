using Apps.Magento.Constants;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Identifiers.Optional;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Magento.DataSources;

public class ContentDataSource : BaseInvocable, IAsyncDataSourceItemHandler
{
    private readonly string _contentType;
    private readonly StoreViewOptionalIdentifier _storeViewIdentifier;

    public ContentDataSource(
        InvocationContext invocationContext,
        [ActionParameter] ContentTypeIdentifier contentTypeIdentifier,
        [ActionParameter] StoreViewOptionalIdentifier storeViewIdentifier) : base(invocationContext)
    {
        string contentType = contentTypeIdentifier.ContentType.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(contentType))
            throw new PluginMisconfigurationException("Please specify a content type first");
        
        _contentType = contentTypeIdentifier.ContentType;
        _storeViewIdentifier = storeViewIdentifier;
    }

    public Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        IAsyncDataSourceItemHandler dataSource = _contentType switch
        {
            ContentTypeConstants.Block => new BlockDataSource(InvocationContext),
            ContentTypeConstants.Page => new PageDataSource(InvocationContext),
            ContentTypeConstants.Product => new ProductDataSource(InvocationContext, _storeViewIdentifier),
            _ => throw new PluginMisconfigurationException(
                $"Content type '{_contentType}' is not supported. Supported: {string.Join(", ", ContentTypeConstants.SupportedContentTypes)}")
        };

        return dataSource.GetDataAsync(context, cancellationToken);
    }
}