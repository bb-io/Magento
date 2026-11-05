using Apps.Magento.Models.Entities.Products;
using Apps.Magento.Utils;
using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Magento.Models.Responses.Products;

public class ProductResponse(ProductEntity entity)
{
    [Display("Product ID")]
    public string Id { get; set; } = entity.Id;

    [Display("Product SKU")]
    public string Sku { get; set; } = entity.Sku;

    [Display("Product name")]
    public string Name { get; set; } = entity.Name;

    [Display("Attribute set ID")]
    public string AttributeSetId { get; set; } = entity.AttributeSetId;

    [Display("Product price")]
    public double Price { get; set; } = entity.Price;

    [Display("Product status")]
    public int Status { get; set; } = entity.Status;

    [Display("Product visibility")]
    public int Visibility { get; set; } = entity.Visibility;

    [Display("Product type ID")]
    public string TypeId { get; set; } = entity.TypeId;

    [Display("Product created at"), JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime CreatedAt { get; set; } = entity.CreatedAt;

    [Display("Product updated at"), JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime UpdatedAt { get; set; } = entity.UpdatedAt;

    [Display("Product weight")]
    public double Weight { get; set; } = entity.Weight;

    [Display("Custom attributes")]
    public List<CustomAttributeResponse> CustomAttributes { get; set; } =
        entity.CustomAttributes.Select(x => new CustomAttributeResponse(x)).ToList();
}