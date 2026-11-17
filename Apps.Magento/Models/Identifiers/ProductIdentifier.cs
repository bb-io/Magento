using Apps.Magento.DataSources;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Magento.Models.Identifiers;

public class ProductIdentifier
{
    [Display("Product SKU"), DataSource(typeof(ProductDataSource))]
    public string Sku { get; set; } = string.Empty;
}