using Apps.Magento.DataSources;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Magento.Models.Identifiers;

public class BlockIdentifier
{
    [Display("Block ID"), DataSource(typeof(BlockDataSource))]
    public string BlockId { get; set; } = string.Empty;
}