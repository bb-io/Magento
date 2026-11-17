using Apps.Magento.Models.Entities.Blocks;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Models.Responses.Blocks;

public class BlockResponse(BlockEntity entity)
{
    [Display("Block ID")]
    public string Id { get; set; } = entity.Id;

    [Display("Block identifier")]
    public string Identifier { get; set; } = entity.Identifier;

    [Display("Block title")]
    public string Title { get; set; } = entity.Title;

    [Display("Block created at")]
    public DateTime CreatedAt { get; set; } = entity.CreationTime;

    [Display("Block updated at")]
    public DateTime UpdatedAt { get; set; } = entity.UpdateTime;

    [Display("Block is active")]
    public bool Active { get; set; } = entity.Active;
}