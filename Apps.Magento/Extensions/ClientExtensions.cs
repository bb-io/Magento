using Apps.Magento.Api;
using Apps.Magento.Models.Entities.Categories;
using Apps.Magento.Models.Entities.Products;
using RestSharp;

namespace Apps.Magento.Extensions;

public static class ClientExtensions
{
    public static Task<CategoryEntity> GetCategory(this ApiClient client, string categoryId)
    {
        var request = new RestRequest($"V1/categories/{categoryId}");
        return client.ExecuteWithErrorHandling<CategoryEntity>(request);
    }

    public static Task<ProductEntity> GetProduct(this ApiClient client, string productSku, string? storeView)
    {
        var request = new RestRequest($"{storeView}/V1/products/{productSku}");
        return client.ExecuteWithErrorHandling<ProductEntity>(request);
    }
}