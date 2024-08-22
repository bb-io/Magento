using System.Text;
using System.Web;
using Apps.Magento.Models;
using Apps.Magento.Models.Responses.Products;
using HtmlAgilityPack;

namespace Apps.Magento.Utils;

public static class HtmlHelper
{
    public static string GenerateProductHtmlContent(ProductResponse product, List<string>? customAttributes)
    {
        var htmlBuilder = new StringBuilder();
        htmlBuilder.Append($"<h1>{product.Name}</h1>");
        var defaultAttributes = new HashSet<string>
        {
            "meta_title", "meta_description", "meta_keyword", "description"
        };

        var attributesToInclude = customAttributes?.ToHashSet() ?? defaultAttributes;
        foreach (var customAttribute in product.CustomAttributes)
        {
            if (attributesToInclude.Contains(customAttribute.AttributeCode))
            {
                htmlBuilder.Append($"<div data-attribute-code=\"{customAttribute.AttributeCode}\">");
                htmlBuilder.Append($"{HttpUtility.HtmlEncode(customAttribute.Value)}");
                htmlBuilder.Append("</div>");
            }
        }

        return htmlBuilder.ToString();
    }
    
    public static ProductModel ParseCustomAttributesFromHtml(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var customAttributes = new List<CustomAttribute>();
        var nameNode = doc.DocumentNode.SelectSingleNode("//h1");
        var name = nameNode?.InnerHtml.Trim() ?? string.Empty;
        
        var attributeNodes = doc.DocumentNode.SelectNodes("//div[@data-attribute-code]");
        if (attributeNodes != null)
        {
            foreach (var attributeNode in attributeNodes)
            {
                var attributeCode = attributeNode.GetAttributeValue("data-attribute-code", string.Empty).Trim();

                if (!string.IsNullOrEmpty(attributeCode))
                {
                    var value = HttpUtility.HtmlDecode(attributeNode.InnerHtml.Trim());
                    customAttributes.Add(new CustomAttribute 
                    { 
                        AttributeCode = attributeCode, 
                        Value = value 
                    });
                }
            }
        }

        return new ProductModel(name, customAttributes);
    } 
    
    public static Stream ConvertToHtml(string contentType, string resourceId, string content)
    {
        var (doc, body) = PrepareEmptyHtmlDocument(contentType, resourceId);

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
    
    private static (HtmlDocument document, HtmlNode bodyNode) PrepareEmptyHtmlDocument(string contentType, string resourceId)
    {
        var htmlDoc = new HtmlDocument();
        var htmlNode = htmlDoc.CreateElement("html");
        htmlDoc.DocumentNode.AppendChild(htmlNode);

        var headNode = htmlDoc.CreateElement("head");
        htmlNode.AppendChild(headNode);

        var contentTypeMetaNode = htmlDoc.CreateElement("meta");
        contentTypeMetaNode.SetAttributeValue("name", "blackbird-content-type");
        contentTypeMetaNode.SetAttributeValue("content", contentType);
        headNode.AppendChild(contentTypeMetaNode);

        var resourceIdMetaNode = htmlDoc.CreateElement("meta");
        resourceIdMetaNode.SetAttributeValue("name", "blackbird-resource-id");
        resourceIdMetaNode.SetAttributeValue("content", resourceId);
        headNode.AppendChild(resourceIdMetaNode);

        var bodyNode = htmlDoc.CreateElement("body");
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
