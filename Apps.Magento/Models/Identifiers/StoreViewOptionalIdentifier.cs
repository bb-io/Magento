using Apps.Magento.DataSources;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Magento.Models.Identifiers;

public class StoreViewOptionalIdentifier  
{
    [Display("Store view", Description = "If you will not specify it it will be empty"), DataSource(typeof(StoreViewDataSource))]
    public string? StoreView { get; set; }

    public override string ToString()
    {
        return StoreView ?? String.Empty;
    }
}