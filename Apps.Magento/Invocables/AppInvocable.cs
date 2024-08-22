using System.Reflection;
using System.Text;
using Apps.Magento.Api;
using Apps.Magento.Models.Requests;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Magento.Invocables;

public class AppInvocable : BaseInvocable
{
    protected AuthenticationCredentialsProvider[] Creds =>
        InvocationContext.AuthenticationCredentialsProviders.ToArray();

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
    
    protected virtual void ValidateFilterRequest(BaseFilterRequest filterRequest)
    {
        var properties = filterRequest.GetType().GetProperties();
        var filledProperties = properties.Where(p => p.GetValue(filterRequest) != null).ToList();

        if (filledProperties.Count > 0 && filledProperties.Count < properties.Length)
        {
            var missingProperties = properties.Except(filledProperties)
                .Select(p => p.GetCustomAttribute<DisplayAttribute>()?.Name ?? p.Name)
                .ToArray();

            throw new ArgumentException($"Missing required filter properties: {string.Join(", ", missingProperties)}");
        }
    }

    protected virtual string BuildQueryString(BaseFilterRequest filterRequest)
    {
        var queryString = new StringBuilder();
        if (!string.IsNullOrEmpty(filterRequest.Title) &&
            !string.IsNullOrEmpty(filterRequest.ConditionType))
        {
            queryString.Append($"[filterGroups][0][filters][0][field]={Uri.EscapeDataString("title")}");
            queryString.Append(
                $"&searchCriteria[filterGroups][0][filters][0][value]={Uri.EscapeDataString(filterRequest.Title)}");
            queryString.Append(
                $"&searchCriteria[filterGroups][0][filters][0][conditionType]={Uri.EscapeDataString(filterRequest.ConditionType)}");
        }

        return queryString.ToString();
    }
}