namespace Apps.Magento.Services.Models;

public record PollContentServiceRequest(
    DateTime UpdatedAfter,
    string? NameContains,
    string? StoreView,
    IReadOnlyDictionary<string, string> PreviousHashes);