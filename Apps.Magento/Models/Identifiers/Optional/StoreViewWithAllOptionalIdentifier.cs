using Apps.Magento.DataSources;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Magento.Models.Identifiers.Optional;

public class StoreViewWithAllOptionalIdentifier
{
    [Display("Store view", Description = "'Default' if empty"), DataSource(typeof(StoreViewWithAllDataSource))]
    public string? StoreView { get; set; }

    public override string ToString() => StoreView ?? "default";
    
    public string ReadStoreView() => StoreView is null or "all" ? "default" : StoreView;
}