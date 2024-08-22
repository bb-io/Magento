using Apps.Magento.DataSources;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Magento.Models.Identifiers;

public class BlockIdentifier
{
    [Display("Block ID", Description = "The unique identifier of the block."), DataSource(typeof(BlockDataSource))]
    public string BlockId { get; set; } = string.Empty;
}