using System.Text;
using System.Security.Cryptography;
using System.Web;

namespace Apps.Magento.Utils;

public static class MagentoAuthorizationHelper
{
    public static string CreateAuthorizationHeader(string httpMethod, string url, string oauthConsumerKey, string oauthNonce, string oauthTimestamp, string oauthToken, string consumerSecret, string tokenSecret)
    {
        var oauthSignatureMethod = "HMAC-SHA256";
        var oauthParams = new Dictionary<string, string>
        {
            { "oauth_consumer_key", oauthConsumerKey },
            { "oauth_nonce", oauthNonce },
            { "oauth_signature_method", oauthSignatureMethod },
            { "oauth_timestamp", oauthTimestamp },
            { "oauth_token", oauthToken }
        };

        var oauthSignature = GenerateOauthSignature(httpMethod, new Uri(url), oauthParams, consumerSecret, tokenSecret);
        var authorizationHeader = new StringBuilder("OAuth ");
        foreach (var param in oauthParams)
        {
            authorizationHeader.AppendFormat("{0}=\"{1}\", ", param.Key, Uri.EscapeDataString(param.Value));
        }
        authorizationHeader.AppendFormat("oauth_signature=\"{0}\"", Uri.EscapeDataString(oauthSignature));

        return authorizationHeader.ToString();
    }
    
    private static string GenerateOauthSignature(string httpMethod, Uri uri, Dictionary<string, string> oauthParams, string consumerSecret, string tokenSecret)
    {
        var queryParams = HttpUtility.ParseQueryString(uri.Query);
        foreach (var key in queryParams.AllKeys)
        {
            if (key is not null)
            {
                oauthParams.Add(key, queryParams[key]);
            }
        }

        var sortedParams = new SortedDictionary<string, string>(oauthParams);
        var parameterString = new StringBuilder();
        foreach (var param in sortedParams)
        {
            parameterString.Append($"{Uri.EscapeDataString(param.Key)}={Uri.EscapeDataString(param.Value)}&");
        }
        parameterString.Length--;

        var signatureBaseString = $"{httpMethod.ToUpper()}&{Uri.EscapeDataString(uri.GetLeftPart(UriPartial.Path))}&{Uri.EscapeDataString(parameterString.ToString())}";
        var signingKey = $"{Uri.EscapeDataString(consumerSecret)}&{Uri.EscapeDataString(tokenSecret)}";

        using var hasher = new HMACSHA256(Encoding.ASCII.GetBytes(signingKey));
        byte[] hashBytes = hasher.ComputeHash(Encoding.ASCII.GetBytes(signatureBaseString));
        return Convert.ToBase64String(hashBytes);
    }

    public static string GenerateTimestamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
    }

    public static string GenerateNonce()
    {
        return Guid.NewGuid().ToString("N");
    }
}