using System.Text;
using Apps.Magento.Constants;
using Apps.Magento.Extensions;
using Apps.Magento.Models;
using Apps.Magento.Models.Utility.Content;
using HtmlAgilityPack;

namespace Apps.Magento.Utils;

public static class HtmlHelper
{
    public static Stream ConvertToHtml(ContentMetadata metadata, string content)
    {
        var (doc, body) = PrepareEmptyHtmlDocument(metadata);

        var contentNode = doc.CreateElement("div");
        contentNode.InnerHtml = content;
        body.AppendChild(contentNode);

        return GetMemoryStream(doc);
    }
    
    public static HtmlModel ConvertFromHtml(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var contentType = doc.DocumentNode
            .SelectSingleNode("//meta[@name='blackbird-content-type']")
            ?.GetAttributeValue("content", null);

        var resourceId = doc.DocumentNode
            .SelectSingleNode("//meta[@name='blackbird-resource-id']")
            ?.GetAttributeValue("content", null);

        var bodyNode = doc.DocumentNode.SelectSingleNode("//body/div");
        var content = bodyNode?.InnerHtml ?? string.Empty;

        return new HtmlModel(contentType, resourceId, content);
    }
    
    private static (HtmlDocument document, HtmlNode bodyNode) PrepareEmptyHtmlDocument(ContentMetadata metadata)
    {
        var htmlDoc = new HtmlDocument();
        var htmlNode = htmlDoc.CreateElement("html");
        htmlNode.SetAttributeValue("lang", metadata.Language);
        htmlDoc.DocumentNode.AppendChild(htmlNode);

        var headNode = htmlDoc.CreateElement("head");
        htmlNode.AppendChild(headNode)
            .AddBlackbirdMeta("content-type", metadata.ContentType)
            .AddBlackbirdMeta("resource-id", metadata.ResourceId)
            .AddBlackbirdMeta("ucid", metadata.Ucid.ToString())
            .AddBlackbirdMeta("content-name", metadata.ContentName)
            .AddBlackbirdMeta("system-name", SystemConstants.Name)
            .AddBlackbirdMeta("system-ref", SystemConstants.Ref)
            .AddBlackbirdMeta("public-url", metadata.PublicUrl);

        var bodyNode = htmlDoc.CreateElement("body");
        bodyNode.SetAttributeValue("its-rev-tool", SystemConstants.Name);
        bodyNode.SetAttributeValue("its-rev-tool-ref", SystemConstants.Ref);
        htmlNode.AppendChild(bodyNode);

        return (htmlDoc, bodyNode);
    }

    private static MemoryStream GetMemoryStream(HtmlDocument doc)
    {
        var memoryStream = new MemoryStream();
        doc.Save(memoryStream, Encoding.UTF8);
        memoryStream.Position = 0;
        return memoryStream;
    }
}
