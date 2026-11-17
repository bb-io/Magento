using System.Net;
using Apps.Magento.Constants;
using Apps.Magento.Models;
using Apps.Magento.Models.Entities.Products;
using HtmlAgilityPack;

namespace Apps.Magento.Utils.Converters;

public static class ProductConverter
{
    private static readonly HashSet<string> DefaultAttributes = ["meta_title", "meta_description", "meta_keyword", "description"];
    private static readonly HashSet<string> SizeLimitedAttributes = ["meta_title", "meta_description"]; // These are max 255 chars
    
    public static string ToHtml(ProductEntity product, List<string>? customAttributes)
    {
        var attributesToInclude = new HashSet<string>(DefaultAttributes);
        if (customAttributes != null)
            attributesToInclude.UnionWith(customAttributes);
        
        var doc = new HtmlDocument();
        var root = doc.DocumentNode;

        var nameNode = doc.CreateElement("h1");
        nameNode.SetAttributeValue(HtmlConstants.FieldAttr, "name");
        nameNode.SetAttributeValue(HtmlConstants.KeyAttr, $"{product.Sku}-name");
        nameNode.SetAttributeValue(HtmlConstants.SizeAttr, HtmlConstants.VarcharLimit);
        nameNode.InnerHtml = WebUtility.HtmlEncode(product.Name);

        foreach (var attribute in product.CustomAttributes.Where(x => attributesToInclude.Contains(x.AttributeCode)))
        {
            var node = doc.CreateElement("div");
            node.SetAttributeValue(HtmlConstants.AttributeCodeAttr, attribute.AttributeCode);
            node.SetAttributeValue(HtmlConstants.KeyAttr, $"{product.Sku}-{attribute.AttributeCode}");
            if (SizeLimitedAttributes.Contains(attribute.AttributeCode))
                node.SetAttributeValue(HtmlConstants.SizeAttr, HtmlConstants.VarcharLimit);
            
            node.InnerHtml = attribute.Value;

            if (node.Descendants().Any(x => x.NodeType == HtmlNodeType.Element))
                node.SetAttributeValue(HtmlConstants.FormatAttr, HtmlConstants.HtmlFormat);
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

        var nameNode = doc.DocumentNode.SelectSingleNode($"//h1[@{HtmlConstants.FieldAttr}='name']") ?? 
                       doc.DocumentNode.SelectSingleNode("//h1");
        
        var name = WebUtility.HtmlDecode(nameNode?.InnerHtml.Trim() ?? string.Empty);

        var attributes = doc.DocumentNode.SelectNodes($"//div[@{HtmlConstants.AttributeCodeAttr}]")?
            .Select(node => new CustomAttributeEntity
            {
                AttributeCode = node.GetAttributeValue(HtmlConstants.AttributeCodeAttr, string.Empty).Trim(),
                Value = node.GetAttributeValue(HtmlConstants.FormatAttr, string.Empty) == HtmlConstants.HtmlFormat
                    ? node.InnerHtml.Trim()
                    : WebUtility.HtmlDecode(node.InnerHtml.Trim())
            })
            .Where(x => !string.IsNullOrEmpty(x.AttributeCode))
            .ToList() ?? [];

        return new ProductModel(name, attributes);
    }
}