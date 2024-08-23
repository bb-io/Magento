using Apps.Magento.Actions;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Requests.Blocks;
using Apps.Magento.Models.Responses.Blocks;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Magento.DataSources;

public class BlockDataSource(InvocationContext invocationContext) : AppInvocable(invocationContext), IAsyncDataSourceHandler
{
    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var actions = new BlockActions(InvocationContext, null!);
        var pages = await actions.GetAllBlocksAsync(new StoreViewOptionalIdentifier(), new FilterBlockRequest
        {
            Title = context.SearchString
        });
        
        return pages.Items
            .ToDictionary(x => x.Id, x => GetBlockName(x, pages.Items));
    }
    
    private string GetBlockName(BlockResponse blockResponse, List<BlockResponse> blocks)
    {
        var blockWithSameName = blocks.FirstOrDefault(x => x.Title == blockResponse.Title && x.Id != blockResponse.Id);
        if (blockWithSameName == null)
        {
            return blockResponse.Title;
        }
        
        return $"{blockResponse.Title} ({blockResponse.Identifier})";
    }
}