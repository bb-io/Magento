using Apps.Magento.Models.Entities.StoreViews;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Models.Responses.StoreViews;

public class StoreViewResponse(StoreViewEntity entity)
{
    [Display("Store view ID")]
    public string Id { get; set; } = entity.Id;

    [Display("Store view code")]
    public string Code { get; set; } = entity.Code;

    [Display("Store view name")]
    public string Name { get; set; } = entity.Name;

    [Display("Store view is active")]
    public bool IsActive { get; set; } = entity.IsActive != 0;
}