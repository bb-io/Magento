using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento.Models.Responses.Products;

public class ProductWithCategoriesResponse : ProductResponse
{
    [Display("Category names")] 
    public List<string> CategoryNames { get; set; } = new();

    public ProductWithCategoriesResponse(ProductResponse response)
    {
        Id = response.Id;
        Sku = response.Sku;
        Name = response.Name;
        AttributeSetId = response.AttributeSetId;
        Price = response.Price;
        Status = response.Status;
        Visibility = response.Visibility;
        TypeId = response.TypeId;
        CreatedAt = response.CreatedAt;
        UpdatedAt = response.UpdatedAt;
        Weight = response.Weight;
        ExtensionAttributes = response.ExtensionAttributes;
        ProductLinks = response.ProductLinks;
        Options = response.Options;
        MediaGalleryEntries = response.MediaGalleryEntries;
        TierPrices = response.TierPrices;
        CustomAttributes = response.CustomAttributes;
    }
}