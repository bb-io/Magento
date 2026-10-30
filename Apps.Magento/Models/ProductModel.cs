using Apps.Magento.Models.Entities.Products;

namespace Apps.Magento.Models;

public record ProductModel(string? Name, List<CustomAttribute> CustomAttributes);