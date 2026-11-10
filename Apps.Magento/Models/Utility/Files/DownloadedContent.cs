namespace Apps.Magento.Models.Utility.Files;

public record DownloadedContent(Stream Stream, string MimeType, string FileName);