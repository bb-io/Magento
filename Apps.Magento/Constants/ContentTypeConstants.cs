namespace Apps.Magento.Constants;

public static class ContentTypeConstants
{
    public const string Page = "page";
    public const string Product = "product";
    public const string Block = "block";

    public static readonly IReadOnlyList<string> SupportedContentTypes = [Page, Product, Block];
}