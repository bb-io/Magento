namespace Apps.Magento.Constants;

public static class ErrorMessages
{
    public static string UrlKeyAlreadyExists(string contentType, string contentId, string storeView)
    {
        return
            $"Can't upload {contentType.ToLowerInvariant()} '{contentId}' to store view '{storeView}': " +
            $"this URL is already used there. Check Marketing -> SEO & Search -> URL Rewrites in Magento " +
            $"for an existing rewrite with this path and remove it if it's outdated.";
    }
}