using System.Net;
using System.Text;
using System.Security.Cryptography;
using Apps.Magento.Extensions;

namespace Apps.Magento.Utils;

public static class MagentoAuthorizationHelper
{
    public static string CreateAuthorizationHeader(
        string httpMethod, 
        string url, 
        string consumerKey,
        string token, 
        string consumerSecret, 
        string tokenSecret)
    {
        var uri = new Uri(url);
        var oauthParams = new List<(string Name, string Value)>
        {
            ("oauth_consumer_key", consumerKey),
            ("oauth_nonce", Guid.NewGuid().ToString("N")),
            ("oauth_signature_method", "HMAC-SHA256"),
            ("oauth_timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
            ("oauth_token", token)
        };
        
        var signature = GenerateSignature(
            httpMethod, 
            uri, 
            oauthParams.Concat(ParseQuery(uri.Query)), 
            consumerSecret, 
            tokenSecret);

        var headerParams = oauthParams
            .Append((Name: "oauth_signature", Value: signature))
            .Select(x => $"{x.Name.Encode()}=\"{x.Value.Encode()}\"");
        
        return $"OAuth {string.Join(", ", headerParams)}";
    }
    
    private static string GenerateSignature(
        string httpMethod, 
        Uri uri, 
        IEnumerable<(string Name, string Value)> parameters, 
        string consumerSecret, 
        string tokenSecret)
    { 
        var baseUrl = uri.GetLeftPart(UriPartial.Path);
        string signatureBaseString = $"{httpMethod.ToUpperInvariant()}&{baseUrl.Encode()}&{NormalizeParameters(parameters).Encode()}";
        string signingKey = $"{consumerSecret.Encode()}&{tokenSecret.Encode()}";
        
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(signingKey));
        byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(signatureBaseString));
        return Convert.ToBase64String(hash);
    }
    
    private static IEnumerable<(string Name, string Value)> ParseQuery(string query)
    {
        return query.TrimStart('?')
            .Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Select(pair => pair.Split('=', 2))
            .Select(parts => (
                Name: WebUtility.UrlDecode(parts[0]),
                Value: parts.Length > 1 ? WebUtility.UrlDecode(parts[1]) : string.Empty));
    }
    
    private static string NormalizeParameters(IEnumerable<(string Name, string Value)> parameters)
    {
        return string.Join("&", parameters
            .Select(p => (Name: p.Name.Encode(), Value: p.Value.Encode()))
            .OrderBy(p => p.Name, StringComparer.Ordinal)
            .ThenBy(p => p.Value, StringComparer.Ordinal)
            .Select(p => $"{p.Name}={p.Value}"));
    }
}