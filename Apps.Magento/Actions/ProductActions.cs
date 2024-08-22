using System.Text;
using Apps.Magento.Api;
using Apps.Magento.Constants;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Requests;
using Apps.Magento.Models.Requests.Products;
using Apps.Magento.Models.Responses.Products;
using Apps.Magento.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;

namespace Apps.Magento.Actions;

[ActionList]
public class ProductActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : AppInvocable(invocationContext)
{
    [Action("Get all products", Description = "Get all products")]
    public async Task<ProductsResponse> GetAllProductsAsync(
        [ActionParameter] StoreViewOptionalIdentifier storeViewIdentifier,
        [ActionParameter] FilterProductRequest filterRequest)
    {
        ValidateFilterRequest(filterRequest);
        var queryString = this.BuildQueryString(filterRequest);
        var requestUrl = $"/rest/{storeViewIdentifier}/V1/products?searchCriteria{queryString}";
        return await Client.ExecuteWithErrorHandling<ProductsResponse>(new ApiRequest(requestUrl, Method.Get, Creds));
    }

    [Action("Get product", Description = "Get product by specified SKU")]
    public async Task<ProductResponse> GetProductBySkuAsync(
        [ActionParameter] StoreViewOptionalIdentifier storeViewIdentifier,
        [ActionParameter] ProductIdentifier identifier)
    {
        return await Client.ExecuteWithErrorHandling<ProductResponse>(
            new ApiRequest($"/rest/{storeViewIdentifier}/V1/products/{identifier.Sku}", Method.Get, Creds));
    }

    [Action("Get product as HTML", Description = "Get product by specified SKU as HTML")]
    public async Task<FileReference> GetProductBySkuAsHtmlAsync(
        [ActionParameter] StoreViewOptionalIdentifier storeViewIdentifier,
        [ActionParameter] ProductIdentifier identifier,
        [ActionParameter] GetProductAsHtmlRequest getProductAsHtmlRequest)
    {
        var product = await GetProductBySkuAsync(storeViewIdentifier, identifier);
        var html = HtmlHelper.GenerateProductHtmlContent(product,
            getProductAsHtmlRequest.CustomAttributes?.ToList() ?? null);
        var htmlStream = HtmlHelper.ConvertToHtml(ContentTypeConstants.Page, identifier.Sku, html);
        return await fileManagementClient.UploadAsync(htmlStream, "text/html", $"{identifier.Sku}.html");
    }

    [Action("Create product", Description = "Create product with specified data")]
    public async Task<ProductResponse> CreateProductAsync(
        [ActionParameter] StoreViewOptionalIdentifier storeViewIdentifier,
        [ActionParameter] CreateProductRequest createProductRequest)
    {
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
                type_id = createProductRequest.TypeId,
                weight = createProductRequest.Weight,
                extension_attributes = new
                {
                    category_links = new List<object>()
                },
                custom_attributes = new List<object>()
            }
        };

        var request = new ApiRequest($"/rest/{storeViewIdentifier}/V1/products", Method.Post, Creds)
            .AddBody(body);
        return await Client.ExecuteWithErrorHandling<ProductResponse>(request);
    }

    [Action("Update product", Description = "Update product by specified SKU")]
    public async Task<ProductResponse> UpdateProductBySkuAsync(
        [ActionParameter] StoreViewOptionalIdentifier storeViewIdentifier,
        [ActionParameter] ProductIdentifier identifier,
        [ActionParameter] UpdateProductRequest updateProductRequest)
    {
        if (updateProductRequest.CustomAttributeKeys != null && updateProductRequest.CustomAttributeValues != null)
        {
            if (updateProductRequest.CustomAttributeKeys.Count() != updateProductRequest.CustomAttributeValues.Count())
            {
                throw new ArgumentException("Custom attribute keys and values count must be equal");
            }
        }

        var product = await GetProductBySkuAsync(storeViewIdentifier, identifier);
        var mergedCustomAttributes = updateProductRequest.CustomAttributeKeys != null &&
                                     updateProductRequest.CustomAttributeValues != null
            ? updateProductRequest.CustomAttributeKeys.Zip(updateProductRequest.CustomAttributeValues,
                (key, value) => new CustomAttribute { AttributeCode = key, Value = value }).ToList()
            : new List<CustomAttribute>();
        var body = new
        {
            product = new
            {
                sku = product.Sku,
                name = updateProductRequest.Name ?? product.Name,
                attribute_set_id = int.Parse(updateProductRequest.AttributeSetId ?? product.AttributeSetId),
                price = updateProductRequest.Price ?? product.Price,
                status = product.Status,
                visibility = product.Visibility,
                type_id = updateProductRequest.TypeId ?? product.TypeId,
                weight = updateProductRequest.Weight ?? product.Weight,
                extension_attributes = new
                {
                    category_links = new List<object>()
                },
                custom_attributes = mergedCustomAttributes
            }
        };

        var request = new ApiRequest($"/rest/{storeViewIdentifier}/V1/products/{identifier.Sku}", Method.Put, Creds)
            .AddBody(body);
        return await Client.ExecuteWithErrorHandling<ProductResponse>(request);
    }

    [Action("Update product as HTML", Description = "Update product by specified SKU with HTML content")]
    public async Task<ProductResponse> UpdateProductBySkuAsHtmlAsync(
        [ActionParameter] StoreViewOptionalIdentifier storeViewIdentifier,
        [ActionParameter] UpdateProductAsHtmlRequest updateProductAsHtmlRequest)
    {
        var htmlStream = await fileManagementClient.DownloadAsync(updateProductAsHtmlRequest.File);
        var html = await new StreamReader(htmlStream).ReadToEndAsync();
        var htmlModel = HtmlHelper.ConvertFromHtml(html);
        
        var productModel = HtmlHelper.ParseCustomAttributesFromHtml(html);
        var productSku = updateProductAsHtmlRequest.ProductSku ?? htmlModel.ResourceId ??
            throw new ArgumentException(
                "Couldn't find product SKU in the HTML document. Please specify it manually in the optional input.");
        
        var product = await GetProductBySkuAsync(storeViewIdentifier, new ProductIdentifier { Sku = productSku });
        foreach (var customAttribute in productModel.CustomAttributes)
        {
            var existingAttribute =
                product.CustomAttributes.FirstOrDefault(x => x.AttributeCode == customAttribute.AttributeCode);
            if (existingAttribute != null)
            {
                existingAttribute.Value = customAttribute.Value;
            }
            else
            {
                product.CustomAttributes.Add(customAttribute);
            }
        }

        var body = new
        {
            product = new
            {
                sku = product.Sku,
                name = productModel.Name ?? product.Name,
                attribute_set_id = int.Parse(product.AttributeSetId),
                price = product.Price,
                status = product.Status,
                visibility = product.Visibility,
                type_id = product.TypeId,
                weight = product.Weight,
                extension_attributes = new
                {
                    category_links = new List<object>()
                },
                custom_attributes = product.CustomAttributes
            }
        };

        var request = new ApiRequest($"/rest/{storeViewIdentifier}/V1/products/{productSku}", Method.Put, Creds)
            .AddBody(body);
        return await Client.ExecuteWithErrorHandling<ProductResponse>(request);
    }

    [Action("Delete product", Description = "Delete product by specified SKU")]
    public async Task DeleteProductBySkuAsync([ActionParameter] StoreViewOptionalIdentifier storeViewIdentifier,
        [ActionParameter] ProductIdentifier identifier)
    {
        await Client.ExecuteWithErrorHandling(
            new ApiRequest($"/rest/{storeViewIdentifier}/V1/products/{identifier.Sku}", Method.Delete, Creds));
    }

    protected override string BuildQueryString(BaseFilterRequest filterRequest)
    {
        var queryString = new StringBuilder();
        if (!string.IsNullOrEmpty(filterRequest.Title) &&
            !string.IsNullOrEmpty(filterRequest.ConditionType))
        {
            queryString.Append($"[filterGroups][0][filters][0][field]={Uri.EscapeDataString("name")}");
            queryString.Append(
                $"&searchCriteria[filterGroups][0][filters][0][value]={Uri.EscapeDataString(filterRequest.Title)}");
            queryString.Append(
                $"&searchCriteria[filterGroups][0][filters][0][conditionType]={Uri.EscapeDataString(filterRequest.ConditionType)}");
        }

        return queryString.ToString();
    }
}