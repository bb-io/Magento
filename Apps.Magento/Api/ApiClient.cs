using Apps.Magento.Constants;
using Apps.Magento.Utils;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Magento.Api;

public class ApiClient(IEnumerable<AuthenticationCredentialsProvider> creds)
    : BlackBirdRestClient(new RestClientOptions { BaseUrl = creds.GetUrl(), ThrowOnAnyError = false })
{
    protected override JsonSerializerSettings JsonSettings => JsonConfig.JsonSettings;

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        return ConfigureException(response);
    }

    private Exception ConfigureException(RestResponse response)
    {
        var errorMessage = $"Status code: {response.StatusCode}, Content: {response.Content}";
        return new Exception(errorMessage);
    }
}