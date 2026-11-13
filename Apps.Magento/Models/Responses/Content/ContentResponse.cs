using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Models.Responses.Content;

public record ContentResponse(
    string ContentId, 
    string ContentType,
    string ContentName,
    DateTime CreatedAt,
    DateTime UpdatedAt) : IDownloadContentInput
{
    [Display("Content ID")]
    public string ContentId { get; set; } = ContentId;

    [Display("Content type")]
    public string ContentType { get; set; } = ContentType;

    [Display("Content name")]
    public string ContentName { get; set; } = ContentName;

    [Display("Content created at")]
    public DateTime CreatedAt { get; set; } = CreatedAt;

    [Display("Content updated at")]
    public DateTime UpdatedAt { get; set; } = UpdatedAt;
}