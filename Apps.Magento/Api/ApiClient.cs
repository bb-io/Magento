using System.Net;
using Apps.Magento.Constants;
using Apps.Magento.Extensions;
using Apps.Magento.Models.Dtos;
using Apps.Magento.Utils;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Magento.Api;

public class ApiClient(IEnumerable<AuthenticationCredentialsProvider> creds)
    : BlackBirdRestClient(new RestClientOptions { BaseUrl = new Uri(creds.GetUrl()), ThrowOnAnyError = false })
{
    protected override JsonSerializerSettings JsonSettings => JsonConfig.JsonSettings;

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        var statusCode = response.StatusCode;
        if (statusCode == HttpStatusCode.InternalServerError)
        {
            return new PluginApplicationException(
                $"The Magento instance returned a server error ({statusCode}). This is a problem on the Magento side. " +
                $"Please check var/log/ and var/report/ on the instance");
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