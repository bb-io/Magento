namespace Apps.Magento.Utils;

public static class StoreViewHelper
{
    public static string ForRead(string? storeView)
    {
        return string.IsNullOrWhiteSpace(storeView) || storeView == "all" ? "default" : storeView;
    }

    public static string ForWrite(string? storeView)
    {
        return string.IsNullOrWhiteSpace(storeView) ? "default" : storeView;
    }
}