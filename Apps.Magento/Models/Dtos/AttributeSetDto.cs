namespace Apps.Magento.Models.Dtos;

public class AttributeSetDto
{
    public int AttributeSetId { get; set; }
    
    public string AttributeSetName { get; set; } = string.Empty;
    
    public int SortOrder { get; set; }
    
    public int EntityTypeId { get; set; }
}