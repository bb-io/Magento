using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Magento.Utils;

public static class ContentResolver
{
    public static string ResolveContentId(
        string? idFromInput,
        string? idFromFile,
        string contentIdName)
    {
        return idFromInput ?? 
               idFromFile ?? 
               throw new PluginMisconfigurationException(
                   $"Couldn't find {contentIdName.Trim()} in the file. Please specify it manually in the optional input.");
    }
}