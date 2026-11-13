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
    
    [Display("Product attribute set ID"), DataSource(typeof(ProductAttributeSetDataSource))]
    public string AttributeSetId { get; set; } = string.Empty;
    
    [Display("Product price")]
    public double Price { get; set; }
    
    [Display("Product type"), DataSource(typeof(ProductTypeDataSource))]
    public string Type { get; set; } = string.Empty;

    [Display("Product weight")]
    public double Weight { get; set; }

    [Display("Price view")]
    public string? PriceView { get; set; } 
}