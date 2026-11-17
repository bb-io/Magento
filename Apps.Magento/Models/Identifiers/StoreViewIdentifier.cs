using Apps.Magento.DataSources;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Magento.Models.Identifiers;

public class StoreViewIdentifier
{
    [Display("Store view"), DataSource(typeof(StoreViewDataSource))]
    public string StoreView { get; set; } = string.Empty;
}