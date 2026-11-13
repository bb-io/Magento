using Apps.Magento.DataSources;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Magento.Models.Identifiers.Optional;

public class StoreViewOptionalIdentifier  
{
    [Display("Store view"), DataSource(typeof(StoreViewDataSource))]
    public string? StoreView { get; set; }

    public override string ToString()
    {
        return StoreView ?? string.Empty;
    }
}