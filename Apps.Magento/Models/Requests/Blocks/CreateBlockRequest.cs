using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Magento.Models.Requests.Blocks;

public class CreateBlockRequest
{
    [Display("Block identifier")]
    public string Identifier { get; set; } = string.Empty;
    
    [Display("Block title")]
    public string Title { get; set; } = string.Empty;

    [Display("Block is active", Description = "True by default")]
    public bool? Active { get; set; }

    [Display("File", Description = "File with the block content")]
    public FileReference? File { get; set; }
}