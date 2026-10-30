using Apps.Magento.Models.Entities.Categories;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Models.Responses.Categories;

public class CategoryResponse(CategoryEntity entity)
{
    [Display("Category ID")]
    public string Id { get; set; } = entity.Id;
    
    [Display("Category name")]
    public string Name { get; set; } = entity.Name;
    
    [Display("Parent ID")]
    public string ParentId { get; set; } = entity.ParentId;

    [Display("Category is active")]
    public bool IsActive { get; set; } = entity.IsActive;
}