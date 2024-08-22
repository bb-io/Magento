using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Magento.DataSources.Static;

public class ConditionTypeStaticDataSource : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
    {
        return new()
        {
            { "eq", "Equal" },
            { "neq", "Not equal" },
            { "like", "Like" },
            { "nlike", "Not like" }
        };
    }
}