using Apps.Magento.Models.Entities.Products;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Models.Responses.Products;

public class CustomAttributeResponse(CustomAttribute entity)
{
    [Display("Attribute code")]
    public string AttributeCode { get; set; } = entity.AttributeCode;

    [Display("Attribute value")]
    public string Value { get; set; } = entity.Value;
}