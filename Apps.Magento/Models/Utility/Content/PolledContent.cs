using Apps.Magento.Models.Responses.Content;

namespace Apps.Magento.Models.Utility.Content;

public record PolledContent(List<ContentResponse> Changed, Dictionary<string, string> Hashes);