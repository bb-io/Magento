using Apps.Magento.Constants;
using Apps.Magento.Utils;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using RestSharp;
using RestSharp.Authenticators;

namespace Apps.Magento.Authenticators;

public class MagentoOAuth1Authenticator(IEnumerable<AuthenticationCredentialsProvider> creds) : IAuthenticator
{
    public ValueTask Authenticate(IRestClient client, RestRequest request)
    {
        var credsList = creds.ToList();
        var url = client.BuildUri(request).AbsoluteUri;

        var authorizationHeader = MagentoAuthorizationHelper.CreateAuthorizationHeader(
            request.Method.ToString(),
            url,
            credsList.Get(CredsNames.ConsumerKey).Value,
            credsList.Get(CredsNames.AccessToken).Value,
            credsList.Get(CredsNames.ConsumerSecret).Value,
            credsList.Get(CredsNames.AccessTokenSecret).Value);

        request.AddOrUpdateHeader("Authorization", authorizationHeader);
        return ValueTask.CompletedTask;
    }
}