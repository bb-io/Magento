using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Models.Requests;

public class BaseFilterRequest
{
    [Display("Title", Description = "You can filter by title")]
    public virtual string? Title { get; set; }

    [Display("Created at", Description = "You can filter by created date")]
    public DateTime? CreatedAt { get; set; }
    
    [Display("Updated at", Description = "You can filter by updated date")]
    public DateTime? UpdatedAt { get; set; }
}