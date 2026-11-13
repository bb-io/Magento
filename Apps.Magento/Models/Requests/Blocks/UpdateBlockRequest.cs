using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Models.Requests.Blocks;

public class UpdateBlockRequest
{
    [Display("Block identifier")]
    public string? Identifier { get; set; }
    
    [Display("Block title")]
    public string? Title { get; set; }

    [Display("Block is active")]
    public bool? Active { get; set; }
}