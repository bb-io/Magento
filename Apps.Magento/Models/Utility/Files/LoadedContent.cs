using Blackbird.Filters.Transformations;

namespace Apps.Magento.Models.Utility.Files;

public record LoadedContent(string Html, Transformation? Transformation, bool WasBilingual);