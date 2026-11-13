using System.Net;
using Apps.Magento.Authenticators;
using Apps.Magento.Constants;
using Apps.Magento.Extensions;
using Apps.Magento.Models.Dtos;
using Apps.Magento.Models.Utility.Pagination;
using Apps.Magento.Utils;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Magento.Api;

public class ApiClient(IEnumerable<AuthenticationCredentialsProvider> creds) 
    : BlackBirdRestClient(new RestClientOptions
    {
        BaseUrl = new Uri($"{creds.GetUrl()}/rest"),
        ThrowOnAnyError = false,
        Authenticator = new MagentoOAuth1Authenticator(creds)
    })
{
    protected override JsonSerializerSettings JsonSettings => JsonConfig.JsonSettings;

    public async Task<IEnumerable<T>> Paginate<T>(RestRequest request, int? maxPages = null)
    {
        const int pageSize = 100;
        var items = new List<T>();

        request.AddOrUpdateParameter(new QueryParameter("searchCriteria[pageSize]", pageSize.ToString(), false));

        for (var page = 1; maxPages is null || page <= maxPages; page++)
        {
            request.AddOrUpdateParameter(new QueryParameter("searchCriteria[currentPage]", page.ToString(), false));

            var result = await ExecuteWithErrorHandling<PaginationResponse<T>>(request);
            items.AddRange(result.Items);

            if (result.Items.Count < pageSize || items.Count >= result.TotalCount)
                break;
        }

        return items;
    }
    
    protected override Exception ConfigureErrorException(RestResponse response)
    {
        var statusCode = response.StatusCode;
        if (statusCode == HttpStatusCode.InternalServerError)
        {
            var details = TryParseError(response.Content);
            return new PluginApplicationException(
                $"The Magento instance returned a server error ({statusCode}). This is a problem on the Magento side. " +
                $"Please check var/log/ and var/report/ on the instance" +
                (details is null ? string.Empty : $". {details}"));
        }
        
        var errorDto = TryParseError(response.Content);
        string message = errorDto is not null
            ? errorDto.ToString()
            : $"Status code: {statusCode}. Content: {response.Content?.SanitizeCurlyBraces()}";

        return new PluginApplicationException(message);
    }
    
    private static ErrorDto? TryParseError(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return null;

        try
        {
            return JsonConvert.DeserializeObject<ErrorDto>(content);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}