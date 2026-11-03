using Apps.Magento.DataSources;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Magento.Models.Requests.StoreViews;

public class CreateStoreViewRequest
{
    [Display("Store view name")]
    public string Name { get; set; } = string.Empty;

    [Display("Store view code")]
    public string Code { get; set; } = string.Empty;

    [Display("Store view locale", Description = "Locale of your new store view. Format: de_DE")]
    public string Locale { get; set; } = string.Empty;

    [Display("Store group ID"), DataSource(typeof(StoreGroupDataSource))]
    public string GroupId { get; set; } = string.Empty;

    [Display("Is active", Description = "True by default")]
    public bool? IsActive { get; set; }
}