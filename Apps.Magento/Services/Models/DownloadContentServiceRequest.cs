namespace Apps.Magento.Services.Models;

public record DownloadContentServiceRequest
{
    public required string ContentId { get; set; }
    public IEnumerable<string>? CustomAttributes { get; set; }
    public string? StoreView { get; set; }
}