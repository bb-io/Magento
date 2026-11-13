using Newtonsoft.Json;

namespace Apps.Magento.Models.Utility.Pagination;

public class SearchCriteriaResponse
{
    [JsonProperty("page_size")]
    public int PageSize { get; set; }

    [JsonProperty("current_page")]
    public int CurrentPage { get; set; }
}