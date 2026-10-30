using Apps.Magento.Models.Entities.Products;

namespace Apps.Magento.Models;

public class ProductModel(string? name, List<CustomAttribute> customAttributes)
{
    public string? Name { get; set; } = name;

    public List<CustomAttribute> CustomAttributes { get; set; } = customAttributes;
}