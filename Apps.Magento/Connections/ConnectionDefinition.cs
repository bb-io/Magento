using Apps.Magento.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.Magento.Connections;

public class ConnectionDefinition : IConnectionDefinition
{
    private static IEnumerable<ConnectionProperty> ConnectionProperties => new[]
    {
        new ConnectionProperty(CredsNames.BaseUrl)
        {
            DisplayName = "Base URL",
            Description = "We expect the base URL to be in the format https://magento-demo.testing.example.com",
            Sensitive = false
        },
        new ConnectionProperty(CredsNames.ConsumerKey)
        {
            DisplayName = "Consumer key",
            Description = "Consumer key for the API",
            Sensitive = true
        },
        new ConnectionProperty(CredsNames.ConsumerSecret)
        {
            DisplayName = "Consumer secret",
            Description = "Consumer secret for the API",
            Sensitive = true
        },
        new ConnectionProperty(CredsNames.AccessToken)
        {
            DisplayName = "Access token",
            Description = "Access token for the API",
            Sensitive = true
        },
        new ConnectionProperty(CredsNames.AccessTokenSecret)
        {
            DisplayName = "Access token secret",
            Description = "Access token secret for the API",
            Sensitive = true
        }
    };

    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups => new List<ConnectionPropertyGroup>
    {
        new()
        {
            Name = "Developer API key",
            AuthenticationType = ConnectionAuthenticationType.Undefined,
            ConnectionUsage = ConnectionUsage.Actions,
            ConnectionProperties = ConnectionProperties
        }
    };

    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(
        Dictionary<string, string> values) =>
        values.Select(x =>
                new AuthenticationCredentialsProvider(AuthenticationCredentialsRequestLocation.None, x.Key, x.Value))
            .ToList();
}