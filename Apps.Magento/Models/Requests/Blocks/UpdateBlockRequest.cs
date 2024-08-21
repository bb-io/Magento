namespace Apps.Magento.Models.Requests.Blocks;

public class UpdateBlockRequest
{
    public string? Identifier { get; set; }
    
    public string? Title { get; set; }
    
    public string? Content { get; set; }

    public bool? Active { get; set; }
}