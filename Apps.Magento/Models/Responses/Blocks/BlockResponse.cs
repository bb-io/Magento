using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Models.Responses.Blocks;

public class BlockResponse
{
    [Display("Block ID")]
    public string Id { get; set; } = string.Empty;
    
    [Display("Identifier")]
    public string Identifier { get; set; } = string.Empty;
    
    [Display("Title")]
    public string Title { get; set; } = string.Empty;
    
    [Display("Content")]
    public string Content { get; set; } = string.Empty;
    
    [Display("Creation time")]
    public DateTime CreationTime { get; set; }
    
    [Display("Update time")]
    public DateTime UpdateTime { get; set; }
    
    [Display("Active")]
    public bool Active { get; set; }
}