using Newtonsoft.Json;

namespace Apps.Magento.Models.Utility.Pagination;

public class PaginationResponse<T>
{
    [JsonProperty("items")]
    public List<T> Items { get; set; } = [];

    [JsonProperty("search_criteria")]
    public SearchCriteriaResponse SearchCriteria { get; set; } = null!;

    [JsonProperty("total_count")]
    public int TotalCount { get; set; }
}