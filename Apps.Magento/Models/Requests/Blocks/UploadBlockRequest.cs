using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Magento.Models.Requests.Blocks;

public class UploadBlockRequest
{
    [Display("File")]
    public FileReference File { get; set; } = null!;
}