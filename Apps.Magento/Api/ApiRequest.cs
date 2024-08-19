using Apps.Magento.Constants;
using Apps.Magento.Utils;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using RestSharp;

namespace Apps.Magento.Api;

public class ApiRequest(string resource, Method method, IEnumerable<AuthenticationCredentialsProvider> creds)
    : BlackBirdRestRequest(resource, method, creds)
{

    protected override void AddAuth(IEnumerable<AuthenticationCredentialsProvider> creds)
    {
        var credentials = creds.ToList();
        var oauthConsumerKey = credentials.Get(CredsNames.ConsumerKey).Value;
        var oauthToken = credentials.Get(CredsNames.AccessToken).Value;
        var consumerSecret = credentials.Get(CredsNames.ConsumerSecret).Value;
        var tokenSecret = credentials.Get(CredsNames.AccessTokenSecret).Value;

        var oauthNonce = MagentoAuthorizationHelper.GenerateNonce();
        var oauthTimestamp = MagentoAuthorizationHelper.GenerateTimestamp();

        var authorizationHeader = MagentoAuthorizationHelper.CreateAuthorizationHeader(Method.ToString(), Resource, oauthConsumerKey, oauthNonce,
            oauthTimestamp, oauthToken, consumerSecret, tokenSecret);
        
        this.AddHeader("Authorization", authorizationHeader);
    }
}