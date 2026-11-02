using System.Collections;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Magento.Utils;

public static class RequestValidator
{
    public static void EnsureAnyFilled(object request)
    {
        var anyFilled = request.GetType()
            .GetProperties()
            .Any(x => IsFilled(x.GetValue(request)));

        if (!anyFilled)
            throw new PluginMisconfigurationException("Please specify at least one field to update");
    }
    
    private static bool IsFilled(object? value) => value switch
    {
        null => false,
        string text => !string.IsNullOrWhiteSpace(text),
        IEnumerable collection => collection.Cast<object>().Any(),
        _ => true
    };
}