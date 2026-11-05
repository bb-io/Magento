using Apps.Magento.Models;
using Apps.Magento.Models.Utility.Files;
using Apps.Magento.Utils;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Magento.Extensions;

public static class FileManagementClientExtensions
{
    public static async Task<HtmlModel> DownloadHtml(this IFileManagementClient fileManagementClient, FileReference file)
    {
        var htmlStream = await fileManagementClient.DownloadAsync(file);
        string html = await new StreamReader(htmlStream).ReadToEndAsync();
        return HtmlHelper.ConvertFromHtml(html);
    }

    public static Task<FileReference> UploadDownloadedFile(this IFileManagementClient fileManagementClient, DownloadedFile file)
    {
        return fileManagementClient.UploadAsync(file.Stream, file.MimeType, file.FileName);
    }
}