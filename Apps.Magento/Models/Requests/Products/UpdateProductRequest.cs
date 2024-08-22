using Apps.Magento.DataSources;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Magento.Models.Requests.Products;

public class UpdateProductRequest
{
    [Display("Product name")]
    public string? Name { get; set; }
    
    [Display("Attribute set ID"), DataSource(typeof(AttributeSetDataSource))]
    public string? AttributeSetId { get; set; }
    
    [Display("Product price")]
    public double? Price { get; set; }
    
    [Display("Product type"), DataSource(typeof(ProductTypeDataSource))]
    public string? TypeId { get; set; }

    public double? Weight { get; set; }

    [Display("Custom attribute keys"), DataSource(typeof(ProductAttributeDataSource))]
    public IEnumerable<string>? CustomAttributeKeys { get; set; }
    
    [Display("Custom attribute values")]
    public IEnumerable<string>? CustomAttributeValues { get; set; }
}