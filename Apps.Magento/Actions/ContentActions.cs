using Apps.Magento.Constants;
using Apps.Magento.Extensions;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Identifiers.Optional;
using Apps.Magento.Models.Requests.Content;
using Apps.Magento.Models.Responses.Content;
using Apps.Magento.Models.Utility.Files;
using Apps.Magento.Services;
using Apps.Magento.Services.Models;
using Apps.Magento.Utils;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Filters.Constants;

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

    [BlueprintActionDefinition(BlueprintAction.UploadContent)]
    [Action("Upload content", Description = "Upload content")]
    public async Task<UploadContentResponse> UploadContent(
        [ActionParameter] ContentTypeOptionalIdentifier contentTypeIdentifier,
        [ActionParameter] UploadContentRequest input)
    {
        var loaded = await fileManagementClient.LoadContent(input.Content);
        if (loaded.Transformation is null)
            InvocationContext.Logger?.LogInformation("Not a Blackbird interoperable file", []);
        
        var htmlModel = HtmlHelper.ConvertFromHtml(loaded.Html);
        
        string? contentType = contentTypeIdentifier.ContentType ?? htmlModel.ContentType;
        if (string.IsNullOrWhiteSpace(contentType))
            throw new PluginMisconfigurationException("Couldn't determine the content type from the file. Specify it in the input");
        
        string contentId = ContentResolver.ResolveContentId(input.ContentId, htmlModel.ResourceId, $"{contentType} ID");
        var service = ContentServiceFactory.Create(contentType, InvocationContext);
        var uploaded = await service.UploadContent(new(contentId, input.Locale, htmlModel.Content));

        var outputFile = await BuildOutputFile(input, loaded, uploaded);
        return new(outputFile);
    }
    
    private async Task<FileReference> BuildOutputFile(UploadContentRequest input, LoadedContent loaded, UploadedContent uploaded)
    {
        if (loaded.Transformation is null)
            return input.Content;

        var storeConfig = await Client.GetStoreConfig(StoreViewHelper.ForRead(input.Locale));
        
        var transformation = loaded.Transformation;
        transformation.TargetSystemReference.ContentId = uploaded.Ucid.ToString();
        transformation.TargetSystemReference.ContentName = uploaded.ContentName;
        transformation.TargetSystemReference.SystemName = SystemConstants.Name;
        transformation.TargetSystemReference.SystemRef = SystemConstants.Ref;
        transformation.TargetLanguage = storeConfig.Language;

        if (loaded.WasBilingual)
            return await fileManagementClient.UploadAsync(transformation.ToStream(), MediaTypes.Xliff2, transformation.BilingualFileName);

        var target = transformation.Target();
        if (!target.Success)
        {
            InvocationContext.Logger?.LogError($"Failed to load target file: {target.Error}", []);
            return input.Content;
        }

        target.Value.SystemReference = transformation.TargetSystemReference;
        return await fileManagementClient.UploadAsync(target.Value.ToStream(), target.Value.OriginalMediaType, target.Value.OriginalName);
    }
}