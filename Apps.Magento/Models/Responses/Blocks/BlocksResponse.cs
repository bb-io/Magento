using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Models.Responses.Blocks;

public class BlocksResponse : PaginationResponse<BlockResponse>
{
    [Display("Blocks")]
    public override List<BlockResponse> Items { get; set; } = new();
}