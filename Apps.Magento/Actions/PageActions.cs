using System.Reflection;
using System.Text;
using Apps.Magento.Api;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Responses.Pages;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Magento.Actions;

[ActionList]
public class PageActions(InvocationContext invocationContext) : AppInvocable(invocationContext)
{
    [Action("Get all pages", Description = "Get all pages")]
    public async Task<PagesResponse> GetAllPagesAsync([ActionParameter] FilterPageRequest filterRequest)
    {
        ValidateFilterRequest(filterRequest);
        var queryString = BuildQueryString(filterRequest);
        var requestUrl = $"/rest/default/V1/cmsPage/search?searchCriteria{queryString}";

        var request = new ApiRequest(requestUrl, Method.Get, Creds);
        return await Client.ExecuteWithErrorHandling<PagesResponse>(request);
    }

    [Action("Get page", Description = "Get page by specified ID")]
    public async Task<PageResponse> GetPageByIdAsync([ActionParameter] PageIdentifier identifier)
    {
        return await Client.ExecuteWithErrorHandling<PageResponse>(
            new ApiRequest($"/rest/default/V1/cmsPage/{identifier.PageId}", Method.Get, Creds));
    }

    private void ValidateFilterRequest(FilterPageRequest filterRequest)
    {
        var properties = filterRequest.GetType().GetProperties();
        var filledProperties = properties.Where(p => p.GetValue(filterRequest) != null).ToList();

        if (filledProperties.Count > 0 && filledProperties.Count < properties.Length)
        {
            var missingProperties = properties.Except(filledProperties)
                .Select(p => p.GetCustomAttribute<DisplayAttribute>()?.Name ?? p.Name)
                .ToArray();

            throw new ArgumentException($"Missing required filter properties: {string.Join(", ", missingProperties)}");
        }
    }

    private string BuildQueryString(FilterPageRequest filterRequest)
    {
        var queryString = new StringBuilder();
        if (!string.IsNullOrEmpty(filterRequest.Title) &&
            !string.IsNullOrEmpty(filterRequest.ConditionType))
        {
            queryString.Append($"[filterGroups][0][filters][0][field]={Uri.EscapeDataString("title")}");
            queryString.Append(
                $"&searchCriteria[filterGroups][0][filters][0][value]={Uri.EscapeDataString(filterRequest.Title)}");
            queryString.Append(
                $"&searchCriteria[filterGroups][0][filters][0][conditionType]={Uri.EscapeDataString(filterRequest.ConditionType)}");
        }

        return queryString.ToString();
    }
}