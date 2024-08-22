using Apps.Magento.DataSources;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Magento.Models.Requests.Blocks;

public class UpdateBlockFromHtmlRequest
{
    [Display("Block ID", Description = "The unique identifier of the block."), DataSource(typeof(BlockDataSource))]
    public string? BlockId { get; set; }
    
    public FileReference File { get; set; } = new();
}