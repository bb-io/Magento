using Apps.Magento.Api;
using Apps.Magento.Constants;
using Apps.Magento.Models.Entities.Blocks;
using Apps.Magento.Models.Entities.Categories;
using Apps.Magento.Models.Entities.Products;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
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

    public static Task<BlockEntity> GetBlock(this ApiClient client, string blockId)
    {
        var request = new RestRequest($"V1/cmsBlock/{blockId}");
        return client.ExecuteWithErrorHandling<BlockEntity>(request);
    }
    
    public static async Task<BlockEntity> UpdateBlock(this ApiClient client, BlockEntity block)
    {
        var body = new
        {
            block = new
            {
                id = block.Id,
                identifier = block.Identifier,
                title = block.Title,
                content = block.Content,
                active = block.Active
            }
        };

        var request = new RestRequest($"V1/cmsBlock/{block.Id}", Method.Put).WithJsonBody(body, JsonConfig.JsonSettings);
        await client.ExecuteWithErrorHandling(request);
        return await client.GetBlock(block.Id); // Refetching so that the UpdatedAt property populates
    }
}