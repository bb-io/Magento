using Apps.Magento.Api;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using RestSharp;

namespace Apps.Magento.Connections;

public class ConnectionValidator: IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        CancellationToken cancellationToken)
    {
        var credentialsProviders = authenticationCredentialsProviders.ToList();
        var apiClient = new ApiClient(credentialsProviders);
        
        try
        {
            await apiClient.ExecuteWithErrorHandling(new ApiRequest("/rest/default/V1/analytics/link", Method.Get, credentialsProviders));
            return new ConnectionValidationResponse
            {
                IsValid = true
            };
        }
        catch (Exception e)
        {
            return new ConnectionValidationResponse
            {
                IsValid = false,
                Message = e.Message
            };
        }
    }
}