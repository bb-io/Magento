namespace Apps.Magento.Models.Utility.Files;

public record DownloadedFile(Stream Stream, string MimeType, string FileName);