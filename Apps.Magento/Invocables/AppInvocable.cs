using Apps.Magento.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Magento.Invocables;

public class AppInvocable : BaseInvocable
{
    protected AuthenticationCredentialsProvider[] Creds => InvocationContext.AuthenticationCredentialsProviders.ToArray();

    protected ApiClient Client { get; }
    
    public AppInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Client = new ApiClient(Creds);
    }

    protected void ValidateRequestIfAllPropertiesAreNullThrowException<T>(T request) where T : class
    {
        var allPropertiesNull = request.GetType()
            .GetProperties()
            .All(prop => prop.GetValue(request) == null);

        if (allPropertiesNull)
        {
            throw new ArgumentException("At least one property must be specified to update the page.");
        }
    }
}