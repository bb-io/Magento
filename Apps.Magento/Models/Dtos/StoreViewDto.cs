namespace Apps.Magento.Models.Dtos;

public class StoreViewDto
{
    public string Id { get; set; } = string.Empty;
    
    public string Code { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
    
    public int WebsiteId { get; set; }
    
    public int StoreGroupId { get; set; }
    
    public int IsActive { get; set; }
}