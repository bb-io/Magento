namespace Apps.Magento.Models.Requests.Blocks;

public class CreateBlockRequest
{
    public string Identifier { get; set; } = string.Empty;
    
    public string Title { get; set; } = string.Empty;
    
    public string Content { get; set; } = string.Empty;
}