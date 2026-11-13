using Apps.Magento.Constants;
using Apps.Magento.Extensions;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Dtos;
using Apps.Magento.Models.Entities.Products;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Identifiers.Optional;
using Apps.Magento.Models.Requests.Products;
using Apps.Magento.Models.Responses;
using Apps.Magento.Models.Responses.Products;
using Apps.Magento.Services.Concrete;
using Apps.Magento.Services.Models;
using Apps.Magento.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;

namespace Apps.Magento.Actions;

[ActionList("Products")]
public class ProductActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : AppInvocable(invocationContext)
{
    [Action("Search products", Description = "Search products by specific criteria")]
    public async Task<SearchProductsResponse> SearchProducts(
        [ActionParameter] StoreViewOptionalIdentifier storeViewIdentifier,
        [ActionParameter] SearchProductsRequest input)
    {
        RestRequest request = new SearchCriteriaBuilder()
            .WhereContains(("name", input.NameContains))
            .WhereLessThan(("created_at", input.CreatedBefore))
            .WhereGreaterThan(("created_at", input.CreatedAfter))
            .WhereLessThan(("updated_at", input.UpdatedBefore))
            .WhereGreaterThan(("updated_at", input.UpdatedAfter))
            .ApplyTo(new RestRequest($"{storeViewIdentifier}/V1/products"));
        
        var products = await Client.Paginate<ProductEntity>(request);

        if (input.CategoryIds != null)
            products = products.Where(x => x.IsInAllCategories(input.CategoryIds));

        var responses = products.Select(x => new ProductResponse(x)).ToList();
        return new(responses);
    }

    [Action("Get product", Description = "Get product by specified SKU")]
    public async Task<ProductWithCategoriesResponse> GetProduct(
        [ActionParameter] StoreViewOptionalIdentifier storeViewIdentifier,
        [ActionParameter] ProductIdentifier identifier)
    {
        var product = await Client.GetProduct(identifier.Sku, storeViewIdentifier.StoreView);

        var getCategoryTasks = product.ExtensionAttributes.CategoryLinks.Select(x => Client.GetCategory(x.CategoryId));
        var categories = await Task.WhenAll(getCategoryTasks);
        var categoryNames = categories.Select(x => x.Name).ToList();
        
        return new(product, categoryNames);
    }

    [Action("Download product", Description = "Download product content")]
    public async Task<FileResponse> DownloadProduct(
        [ActionParameter] StoreViewOptionalIdentifier storeViewIdentifier,
        [ActionParameter] ProductIdentifier productIdentifier,
        [ActionParameter] DownloadProductRequest downloadProductRequest)
    {
        var service = new ProductService(InvocationContext);
        var downloadRequest = new DownloadContentServiceRequest
        {
            ContentId = productIdentifier.Sku,
            CustomAttributes = downloadProductRequest.CustomAttributes,
            StoreView = storeViewIdentifier.StoreView
        };
        var downloadedFile = await service.DownloadContent(downloadRequest);
        
        var file = await fileManagementClient.UploadDownloadedFile(downloadedFile);
        return new(file);
    }

    [Action("Create product", Description = "Create a new product")]
    public async Task<ProductResponse> CreateProduct([ActionParameter] CreateProductRequest createProductRequest)
    {
        var customAttributes = new List<object>();
        if (!string.IsNullOrEmpty(createProductRequest.PriceView))
        {
            customAttributes.Add(new
            {
                attribute_code = "price_view",
                value = createProductRequest.PriceView
            });
        }
        
        var body = new
        {
            product = new
            {
                sku = createProductRequest.Sku,
                name = createProductRequest.Name,
                attribute_set_id = int.Parse(createProductRequest.AttributeSetId),
                price = createProductRequest.Price,
                status = 1,
                visibility = 1,
                type_id = createProductRequest.Type,
                weight = createProductRequest.Weight,
                extension_attributes = new
                {
                    category_links = new List<object>()
                },
                custom_attributes = customAttributes
            }
        };

        var request = new RestRequest("V1/products", Method.Post).AddBody(body);
        var response = await Client.ExecuteWithErrorHandling<ProductEntity>(request);
        return new(response);
    }

    [Action("Update product", Description = "Update product by specified SKU")]
    public async Task<ProductResponse> UpdateProduct(
        [ActionParameter] StoreViewWithAllOptionalIdentifier storeViewIdentifier,
        [ActionParameter] ProductIdentifier identifier,
        [ActionParameter] UpdateProductRequest input)
    {
        input.Validate();

        var customAttributes = input.CustomAttributeKeys?
            .Zip(input.CustomAttributeValues!, (key, value) => new CustomAttributeDto(key, value))
            .ToList();

        var body = new
        {
            product = new
            {
                sku = identifier.Sku,
                name = input.Name,
                attribute_set_id = input.AttributeSetId is null ? (int?)null : int.Parse(input.AttributeSetId),
                price = input.Price,
                type_id = input.TypeId,
                weight = input.Weight,
                custom_attributes = customAttributes
            }
        };

        string endpoint = $"{StoreViewHelper.ForWrite(storeViewIdentifier.StoreView)}/V1/products/{identifier.Sku}";
        var request = new RestRequest(endpoint, Method.Put)
            .WithJsonBody(body, JsonConfig.JsonSettings);

        var response = await Client.ExecuteWithErrorHandling<ProductEntity>(request);
        return new(response);
    }

    [Action("Upload product", Description = "Upload content of a specific product")]
    public async Task UploadProduct(
        [ActionParameter] StoreViewWithAllOptionalIdentifier storeViewIdentifier,
        [ActionParameter] UploadProductRequest input,
        [ActionParameter] ProductOptionalIdentifier productIdentifier)
    {
        var htmlModel = await fileManagementClient.DownloadHtml(input.File);
        string sku = ContentResolver.ResolveContentId(productIdentifier.Sku, htmlModel.ResourceId, "product SKU");

        var service = new ProductService(InvocationContext);
        var request = new UploadContentServiceRequest(sku, storeViewIdentifier.StoreView, htmlModel.Content);
        await service.UploadContent(request);
    }

    [Action("Delete product", Description = "Delete product by specified SKU")]
    public Task DeleteProduct([ActionParameter] ProductIdentifier identifier)
    {
        var request = new RestRequest($"V1/products/{identifier.Sku}", Method.Delete);
        return Client.ExecuteWithErrorHandling(request);
    }
    
    [Action("Add custom attribute", Description = "Add custom attribute to product by specified SKU")]
    public async Task<ProductResponse> AddCustomAttributeToProduct(
        [ActionParameter] StoreViewOptionalIdentifier storeViewIdentifier,
        [ActionParameter] ProductIdentifier identifier,
        [ActionParameter] AddCustomAttributeRequest input)
    {
        var body = new
        {
            product = new
            {
                sku = identifier.Sku,
                custom_attributes = new[] { new CustomAttributeDto(input.AttributeCode, input.Value) }
            }
        };

        var request = new RestRequest($"{storeViewIdentifier}/V1/products/{identifier.Sku}", Method.Put)
            .WithJsonBody(body, JsonConfig.JsonSettings);

        var response = await Client.ExecuteWithErrorHandling<ProductEntity>(request);
        return new(response);
    }
}