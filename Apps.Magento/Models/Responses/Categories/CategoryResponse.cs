using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Models.Responses.Categories;

public class CategoryResponse
{
    [Display("Category ID")] 
    public string Id { get; set; } = string.Empty;
    
    [Display("Parent ID")]
    public string ParentId { get; set; } = string.Empty;

    [Display("Name")] 
    public string Name { get; set; } = string.Empty;

    [Display("Is active")]
    public bool IsActive { get; set; }
}