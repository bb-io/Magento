using Apps.Magento.Models.Responses;

namespace Apps.Magento.Models.Dtos;

public class ProductAttributesDto : PaginationResponse<ProductAttributeDto>
{ }

public class ProductAttributeDto
{
    public string AttributeCode { get; set; } = string.Empty;

    public string DefaultFrontendLabel { get; set; } = string.Empty;
}