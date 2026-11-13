using Apps.Magento.Utils;

namespace Apps.Magento.Models.Utility.Content;

public record ContentMetadata(
    string ContentType, 
    string ResourceId,
    string Language,
    Ucid Ucid,
    string ContentName,
    string? PublicUrl = null);