using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Magento.Models.Responses.Content;

public record UploadContentResponse(FileReference Content)
{
    [Display("Content")]
    public FileReference Content { get; set; } = Content;
}