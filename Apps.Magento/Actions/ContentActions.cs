using Apps.Magento.Constants;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Identifiers.Optional;
using Apps.Magento.Models.Requests.Content;
using Apps.Magento.Models.Responses.Content;
using Apps.Magento.Services;
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
}