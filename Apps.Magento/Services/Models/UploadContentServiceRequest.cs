namespace Apps.Magento.Services.Models;

public record UploadContentServiceRequest(string ContentId, string? StoreView, string Html);