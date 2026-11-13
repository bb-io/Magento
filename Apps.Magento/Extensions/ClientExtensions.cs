using Apps.Magento.Api;
using Apps.Magento.Constants;
using Apps.Magento.Models.Entities.Blocks;
using Apps.Magento.Models.Entities.Categories;
using Apps.Magento.Models.Entities.Pages;
using Apps.Magento.Models.Entities.Products;
using Apps.Magento.Models.Entities.StoreViews;
using Blackbird.Applications.Sdk.Common.Exceptions;
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
    
    public static async Task<BlockEntity> UpdateBlock(this ApiClient client, BlockEntity block, string? storeCode = null)
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

        var request = new RestRequest($"{storeCode}/V1/cmsBlock/{block.Id}", Method.Put).WithJsonBody(body, JsonConfig.JsonSettings);
        await client.ExecuteWithErrorHandling(request);
        return await client.GetBlock(block.Id); // Refetching so that the UpdatedAt property populates
    }
    
    public static async Task<BlockEntity> CreateBlock(this ApiClient client, BlockEntity block, string storeCode)
    {
        var body = new
        {
            block = new
            {
                identifier = block.Identifier,
                title = block.Title,
                content = block.Content,
                active = block.Active
            }
        };

        var request = new RestRequest($"{storeCode}/V1/cmsBlock", Method.Post).WithJsonBody(body, JsonConfig.JsonSettings);

        var created = await client.ExecuteWithErrorHandling<BlockEntity>(request);
        return await client.GetBlock(created.Id);
    }

    public static Task<PageEntity> GetPage(this ApiClient client, string pageId)
    {
        var request = new RestRequest($"V1/cmsPage/{pageId}");
        return client.ExecuteWithErrorHandling<PageEntity>(request);
    }
    
    public static async Task<PageEntity> UpdatePage(this ApiClient client, PageEntity page, string? storeCode = null)
    {
        var body = new
        {
            page = new
            {
                id = page.Id,
                identifier = page.Identifier,
                title = page.Title,
                page_layout = page.PageLayout,
                meta_title = page.MetaTitle,
                meta_keywords = page.MetaKeywords,
                meta_description = page.MetaDescription,
                content_heading = page.ContentHeading,
                content = page.Content,
                sort_order = page.SortOrder,
                active = page.Active 
            }
        };

        var request = new RestRequest($"{storeCode}/V1/cmsPage/{page.Id}", Method.Put).WithJsonBody(body, JsonConfig.JsonSettings);
        await client.ExecuteWithErrorHandling(request);
        return await client.GetPage(page.Id);   // Refetching so that the UpdatedAt property populates
    }
    
    public static async Task<PageEntity> CreatePage(this ApiClient client, PageEntity page, string storeCode)
    {
        var body = new
        {
            page = new
            {
                identifier = page.Identifier,
                title = page.Title,
                page_layout = page.PageLayout,
                meta_title = page.MetaTitle,
                meta_keywords = page.MetaKeywords,
                meta_description = page.MetaDescription,
                content_heading = page.ContentHeading,
                content = page.Content,
                active = page.Active
            }
        };

        var request = new RestRequest($"{storeCode}/V1/cmsPage", Method.Post).WithJsonBody(body, JsonConfig.JsonSettings);
        var created = await client.ExecuteWithErrorHandling<PageEntity>(request);
        return await client.GetPage(created.Id);    // Refetching so that timestamps populate
    }

    public static async Task<IEnumerable<StoreViewEntity>> ListStoreViews(this ApiClient client)
    {
        var request = new RestRequest("V1/store/storeViews");
        var storeViews = await client.ExecuteWithErrorHandling<IEnumerable<StoreViewEntity>>(request);
        return storeViews.Where(x => x.Code != "admin");
    }

    public static async Task<StoreViewEntity> GetStoreView(this ApiClient client, string viewCode)
    {
        var storeViews = await client.ListStoreViews();
        return storeViews.FirstOrDefault(x => x.Code == viewCode) ??
               throw new PluginMisconfigurationException($"Store view '{viewCode}' was not found");
    }
    
    public static async Task<StoreConfigEntity> GetStoreConfig(this ApiClient client, string storeCode)
    {
        var request = new RestRequest("V1/store/storeConfigs");
        var configs = await client.ExecuteWithErrorHandling<List<StoreConfigEntity>>(request);

        return configs.FirstOrDefault(x => x.Code == storeCode) ?? 
               throw new PluginMisconfigurationException($"Store view '{storeCode}' was not found");
    }
}