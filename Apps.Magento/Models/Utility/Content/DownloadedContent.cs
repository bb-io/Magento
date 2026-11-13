namespace Apps.Magento.Models.Utility.Content;

public record DownloadedContent(Stream Stream, string MimeType, string FileName);