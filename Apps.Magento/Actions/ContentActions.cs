using Apps.Magento.Constants;
using Apps.Magento.Extensions;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Identifiers.Optional;
using Apps.Magento.Models.Requests.Content;
using Apps.Magento.Models.Responses.Content;
using Apps.Magento.Services;
using Apps.Magento.Services.Models;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Magento.Actions;

[ActionList("Content")]
public class ContentActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
    : AppInvocable(invocationContext)
{
    [BlueprintActionDefinition(BlueprintAction.SearchContent)]
    [Action("Search content", Description = "Search items of supported content types by specific criteria")]
    public async Task<SearchContentResponse> SearchContent(
        [ActionParameter] ContentTypesOptionalIdentifier contentTypesIdentifier,
        [ActionParameter] SearchContentRequest input)
    {
        var contentTypes = contentTypesIdentifier.ContentTypes ?? ContentTypeConstants.SupportedContentTypes;
        var services = ContentServiceFactory.CreateMultiple(contentTypes.Distinct(), InvocationContext);

        var searchTasks = services.Select(x => x.SearchContent(input));
        var searchResults = await Task.WhenAll(searchTasks);
        var resultsList = searchResults.SelectMany(x => x).ToList();

        return new(resultsList);
    }

    [BlueprintActionDefinition(BlueprintAction.DownloadContent)]
    [Action("Download content", Description = "Download content of a specific content item")]
    public async Task<DownloadContentResponse> DownloadContent(
        [ActionParameter] ContentTypeIdentifier contentTypeIdentifier,
        [ActionParameter] DownloadContentRequest input,
        [ActionParameter] StoreViewOptionalIdentifier storeViewIdentifier)
    {
        var service = ContentServiceFactory.Create(contentTypeIdentifier.ContentType, InvocationContext);
        var downloadRequest = new DownloadContentServiceRequest
        {
            ContentId = input.ContentId,
            CustomAttributes = input.CustomAttributes,
            StoreView = storeViewIdentifier.StoreView
        };
        var downloadedFile = await service.DownloadContent(downloadRequest);
        
        var file = await fileManagementClient.UploadDownloadedFile(downloadedFile);
        return new(file);
    }
}