using Blackbird.Filters.Transformations;

namespace Apps.Magento.Models.Utility.Content;

public record LoadedContent(string Html, Transformation? Transformation, bool WasBilingual);