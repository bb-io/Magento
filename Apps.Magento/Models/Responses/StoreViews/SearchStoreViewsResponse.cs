using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Models.Responses.StoreViews;

public record SearchStoreViewsResponse([property: Display("Store views")] List<StoreViewResponse> StoreViews);