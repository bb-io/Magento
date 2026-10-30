using System.Net;
using Apps.Magento.Models;
using Apps.Magento.Models.Entities.Products;
using HtmlAgilityPack;

namespace Apps.Magento.Utils.Converters;

public static class ProductConverter
{
    private const string AttributeCodeAttr = "data-attribute-code";
    private const string FormatAttr = "data-format";
    private const string HtmlFormat = "html";
    private const string FieldAttr = "data-field";
    
    private static readonly HashSet<string> DefaultAttributes = ["meta_title", "meta_description", "meta_keyword", "description"];
    
    public static string ToHtml(ProductEntity product, List<string>? customAttributes)
    {
        var attributesToInclude = new HashSet<string>(DefaultAttributes);
        if (customAttributes != null)
            attributesToInclude.UnionWith(customAttributes);
        
        var doc = new HtmlDocument();
        var root = doc.DocumentNode;

        var nameNode = doc.CreateElement("h1");
        nameNode.SetAttributeValue(FieldAttr, "name");
        nameNode.InnerHtml = WebUtility.HtmlEncode(product.Name);
        root.AppendChild(nameNode);

        foreach (var attribute in product.CustomAttributes.Where(x => attributesToInclude.Contains(x.AttributeCode)))
        {
            var node = doc.CreateElement("div");
            node.SetAttributeValue(AttributeCodeAttr, attribute.AttributeCode);
            node.InnerHtml = attribute.Value;

            if (node.Descendants().Any(x => x.NodeType == HtmlNodeType.Element))
                node.SetAttributeValue(FormatAttr, HtmlFormat);
            else
                node.InnerHtml = WebUtility.HtmlEncode(attribute.Value);

            root.AppendChild(node);
        }

        return root.InnerHtml;
    }
    
    public static ProductModel FromHtml(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var nameNode = doc.DocumentNode.SelectSingleNode($"//h1[@{FieldAttr}='name']") ?? 
                       doc.DocumentNode.SelectSingleNode("//h1");
        
        var name = WebUtility.HtmlDecode(nameNode?.InnerHtml.Trim() ?? string.Empty);

        var attributes = doc.DocumentNode.SelectNodes($"//div[@{AttributeCodeAttr}]")?
            .Select(node => new CustomAttribute
            {
                AttributeCode = node.GetAttributeValue(AttributeCodeAttr, string.Empty).Trim(),
                Value = node.GetAttributeValue(FormatAttr, string.Empty) == HtmlFormat
                    ? node.InnerHtml.Trim()
                    : WebUtility.HtmlDecode(node.InnerHtml.Trim())
            })
            .Where(x => !string.IsNullOrEmpty(x.AttributeCode))
            .ToList() ?? [];

        return new ProductModel(name, attributes);
    }
}