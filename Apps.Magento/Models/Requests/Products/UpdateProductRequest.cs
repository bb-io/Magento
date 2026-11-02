using Apps.Magento.DataSources;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Magento.Models.Requests.Products;

public class UpdateProductRequest
{
    [Display("Product name")]
    public string? Name { get; set; }
    
    [Display("Product attribute set ID"), DataSource(typeof(ProductAttributeSetDataSource))]
    public string? AttributeSetId { get; set; }
    
    [Display("Product price")]
    public double? Price { get; set; }
    
    [Display("Product type"), DataSource(typeof(ProductTypeDataSource))]
    public string? TypeId { get; set; }

    [Display("Product weight")]
    public double? Weight { get; set; }

    [Display("Custom attribute keys"), DataSource(typeof(ProductAttributeDataSource))]
    public IEnumerable<string>? CustomAttributeKeys { get; set; }
    
    [Display("Custom attribute values", Description = "Corresponds to the 'Custom attribute keys' input")]
    public IEnumerable<string>? CustomAttributeValues { get; set; }

    public void Validate()
    {
        if (CustomAttributeKeys?.Count() != CustomAttributeValues?.Count())
            throw new PluginMisconfigurationException("Custom attribute keys and values count must be equal");
        
        bool hasAnyInput = !string.IsNullOrWhiteSpace(Name) || 
                           !string.IsNullOrWhiteSpace(AttributeSetId) || 
                           Price.HasValue || 
                           !string.IsNullOrWhiteSpace(TypeId) || 
                           Weight.HasValue || 
                           CustomAttributeKeys?.Any() == true;

        if (!hasAnyInput)
            throw new PluginMisconfigurationException("Please specify at least one product field to update");
    }
}