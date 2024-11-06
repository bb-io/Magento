using Apps.Magento.Api;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Magento.DataSources;

public class AttributeSetDataSource(InvocationContext invocationContext) : AppInvocable(invocationContext), IAsyncDataSourceHandler
{
    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new ApiRequest("/rest/V1/eav/attribute-sets/list?searchCriteria", Method.Get, Creds);
        var storeViews = await Client.ExecuteWithErrorHandling<AttributeSetPaginationDto>(request);
        return storeViews.Items
            .Where(x => context.SearchString == null || BuildReadableName(x).Contains(context.SearchString))
            .ToDictionary(x => x.AttributeSetId.ToString(), BuildReadableName);
    }

    private string BuildReadableName(AttributeSetDto dto) =>
        $"{dto.AttributeSetName} [Sort order: {dto.SortOrder}] [Entity type: {dto.EntityTypeId}]";
}