using Apps.Magento.Api;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using Blackbird.Applications.Sdk.Common.Exceptions;
using RestSharp;

namespace Apps.Magento.Connections;

public class ConnectionValidator : IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        CancellationToken cancellationToken)
    {
        var credentialsProviders = authenticationCredentialsProviders.ToList();
        var apiClient = new ApiClient(credentialsProviders);
        
        try
        {
            await apiClient.ExecuteWithErrorHandling(new ApiRequest("/rest/V1/store/storeViews", Method.Get, credentialsProviders));
            return new ConnectionValidationResponse
            {
                IsValid = true
            };
        }
        catch (PluginApplicationException e)
        {
            return new ConnectionValidationResponse
            {
                IsValid = false,
                Message = e.Message
            };
        }
    }
}