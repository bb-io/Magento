using Apps.Magento.DataSources;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Magento.Models.Requests.Products;

public class AddCustomAttributeRequest
{
    [Display("Custom attribute code"), DataSource(typeof(ProductAttributeDataSource))]
    public string AttributeCode { get; set; } = string.Empty;
    
    [Display("Custom attribute value")]
    public string Value { get; set; } = string.Empty;
}