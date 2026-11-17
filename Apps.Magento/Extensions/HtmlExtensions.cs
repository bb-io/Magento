using HtmlAgilityPack;

namespace Apps.Magento.Extensions;

public static class HtmlExtensions
{
    public static HtmlNode AddBlackbirdMeta(this HtmlNode headNode, string name, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return headNode;

        var metaNode = headNode.OwnerDocument.CreateElement("meta");
        metaNode.SetAttributeValue("name", $"blackbird-{name}");
        metaNode.SetAttributeValue("content", value);
        headNode.AppendChild(metaNode);

        return headNode;
    }
}