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
    private readonly string? _query = resource.Contains("?") 
        ? resource.Substring(resource.IndexOf("?", StringComparison.Ordinal) + 1) 
        : null;

    protected override void AddAuth(IEnumerable<AuthenticationCredentialsProvider> creds)
    {
        var credentials = creds.ToList();
        var baseUrl = credentials.GetUrl();
        var oauthConsumerKey = credentials.Get(CredsNames.ConsumerKey).Value;
        var oauthToken = credentials.Get(CredsNames.AccessToken).Value;
        var consumerSecret = credentials.Get(CredsNames.ConsumerSecret).Value;
        var tokenSecret = credentials.Get(CredsNames.AccessTokenSecret).Value;

        var oauthNonce = MagentoAuthorizationHelper.GenerateNonce();
        var oauthTimestamp = MagentoAuthorizationHelper.GenerateTimestamp();

        var url = baseUrl + Resource;
        if(_query != null)
        {
            url += "?" + _query;
        }
        
        var authorizationHeader = MagentoAuthorizationHelper.CreateAuthorizationHeader(Method.ToString(), url, oauthConsumerKey, oauthNonce,
            oauthTimestamp, oauthToken, consumerSecret, tokenSecret);
        
        this.AddHeader("Authorization", authorizationHeader);
    }
}