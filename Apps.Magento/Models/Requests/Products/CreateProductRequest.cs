using Apps.Magento.DataSources;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Magento.Models.Requests.Products;

public class CreateProductRequest
{
    [Display("Product SKU")]
    public string Sku { get; set; } = string.Empty;
    
    [Display("Product name")]
    public string Name { get; set; } = string.Empty;
    
    [Display("Attribute set ID"), DataSource(typeof(AttributeSetDataSource))]
    public string AttributeSetId { get; set; } = string.Empty;
    
    [Display("Product price")]
    public double Price { get; set; }
    
    [Display("Product type"), DataSource(typeof(ProductTypeDataSource))]
    public string TypeId { get; set; } = string.Empty;

    public double Weight { get; set; }

    [Display("Price view")]
    public string? PriceView { get; set; } 
}