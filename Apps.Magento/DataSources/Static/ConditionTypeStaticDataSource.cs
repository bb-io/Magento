using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Magento.DataSources.Static;

public class ConditionTypeStaticDataSource : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
    {
        return new()
        {
            { "eq", "Equal" },
            { "finset", "A value within a set of values" },
            { "from", "The beginning of a range. Must be used with to." },
            { "gt", "Greater than" },
            { "gteq", "Greater than or equal" },
            { "in", "In. The value can contain a comma-separated list of values." },
            { "like", "Like. The value can contain the SQL wildcard characters when like is specified." },
            { "lt", "Less than" },
            { "lteq", "Less than or equal" },
            { "moreq", "More or equal" },
            { "neq", "Not equal" },
            { "nfinset", "A value that is not within a set of values." },
            { "nin", "Not in. The value can contain a comma-separated list of values." },
            { "nlike", "Not like" },
            { "notnull", "Not null" },
            { "null", "Null" },
            { "to", "The end of a range. Must be used with from." }
        };
    }
}