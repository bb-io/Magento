using Apps.Magento.DataSources;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Magento.Models.Identifiers;

public class StoreViewWithAllOptionalIdentifier
{
    [Display("Store view", Description = "If you will not specify it it will be default"), DataSource(typeof(StoreViewWithAllDataSource))]
    public string? StoreView { get; set; }

    public override string ToString()
    {
        return StoreView ?? "default";
    }
}