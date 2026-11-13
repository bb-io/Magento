using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Magento.Models.Responses.Content;

public record DownloadContentResponse(FileReference Content) : IDownloadContentOutput
{
    [Display("Content")]
    public FileReference Content { get; set; } = Content;
}