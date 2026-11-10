using Apps.Magento.Models;
using Apps.Magento.Models.Utility.Files;
using Apps.Magento.Utils;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Filters.Transformations;

namespace Apps.Magento.Extensions;

public static class FileManagementClientExtensions
{
    public static async Task<HtmlModel> DownloadHtml(this IFileManagementClient fileManagementClient, FileReference file)
    {
        var loaded = await fileManagementClient.LoadContent(file);
        return HtmlHelper.ConvertFromHtml(loaded.Html);
    }
    
    public static async Task<LoadedContent> LoadContent(this IFileManagementClient fileManagementClient, FileReference fileReference)
    {
        await using var file = await fileManagementClient.DownloadAsync(fileReference);
        var buffer = new MemoryStream();
        await file.CopyToAsync(buffer);
        buffer.Position = 0;

        var transformationResult = Transformation.Load(buffer, fileReference.Name, fileReference.ContentType);
        if (transformationResult.Success)
        {
            var target = transformationResult.Value.Target();
            if (target.Success)
                return new(target.Value.ToStream().ReadString(), transformationResult.Value, transformationResult.WasBilingual);
        }

        buffer.Position = 0;
        return new(buffer.ReadString(), null, false);
    }

    public static Task<FileReference> UploadDownloadedFile(this IFileManagementClient fileManagementClient, DownloadedContent file)
    {
        return fileManagementClient.UploadAsync(file.Stream, file.MimeType, file.FileName);
    }
}