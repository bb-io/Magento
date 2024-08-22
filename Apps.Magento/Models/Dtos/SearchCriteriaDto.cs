using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Models.Dtos;

public class SearchCriteriaDto
{
    [DefinitionIgnore]
    public List<FilterGroup> FilterGroups { get; set; } = new();
}

public class FilterGroup
{
}
