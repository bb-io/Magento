using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Magento.Models.Requests.Pages;

public class UploadPageRequest
{
    [Display("File")]
    public FileReference File { get; set; } = null!;
}