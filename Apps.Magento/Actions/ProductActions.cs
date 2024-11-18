using System.Text;
using Apps.Magento.Api;
using Apps.Magento.Constants;
using Apps.Magento.Invocables;
using Apps.Magento.Models.Dtos;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Requests;
using Apps.Magento.Models.Requests.Products;
using Apps.Magento.Models.Responses.Categories;
using Apps.Magento.Models.Responses.Products;
using Apps.Magento.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Magento.Actions;

[ActionList]
public class ProductActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : AppInvocable(invocationContext)
{
    [Action("Search products", Description = "Retrieve all products that match the specified criteria")]
    public async Task<ProductsResponse> GetAllProductsAsync(
        [ActionParameter] StoreViewOptionalIdentifier storeViewIdentifier,
        [ActionParameter] FilterProductRequest filterRequest)
    {
        var queryString = BuildQueryString(filterRequest);
        var requestUrl = $"/rest/{storeViewIdentifier}/V1/products?searchCriteria{queryString}";
        return await Client.ExecuteWithErrorHandling<ProductsResponse>(new ApiRequest(requestUrl, Method.Get, Creds));
    }

    [Action("Get product", Description = "Get product by specified SKU")]
    public async Task<ProductWithCategoriesResponse> GetProductBySkuAsync(
        [ActionParameter] StoreViewOptionalIdentifier storeViewIdentifier,
        [ActionParameter] ProductIdentifier identifier)
    {
        var product = await Client.ExecuteWithErrorHandling<ProductResponse>(
            new ApiRequest($"/rest/{storeViewIdentifier}/V1/products/{identifier.Sku}", Method.Get, Creds));
        
        var categoryActions = new CategoryActions(InvocationContext);
        var categories = new CategoriesResponse();
        
        foreach(var category in product.ExtensionAttributes.CategoryLinks)
        {
            var categoryResponse = await categoryActions.GetCategoryAsync(new()
            {
                CategoryId = category.CategoryId
            });
            categories.Items.Add(categoryResponse);
        }
        
        return new ProductWithCategoriesResponse(product)
        {
            CategoryNames = categories.Items.Select(x => x.Name).ToList()
        };
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
    public async Task<ProductResponse> CreateProductAsync([ActionParameter] CreateProductRequest createProductRequest)
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
                type_id = createProductRequest.TypeId,
                weight = createProductRequest.Weight,
                extension_attributes = new
                {
                    category_links = new List<object>()
                },
                custom_attributes = customAttributes
            }
        };

        var request = new ApiRequest("/rest/V1/products", Method.Post, Creds)
            .AddBody(body);
        return await Client.ExecuteWithErrorHandling<ProductResponse>(request);
    }

    [Action("Update product", Description = "Update product by specified SKU")]
    public async Task<ProductResponse> UpdateProductBySkuAsync(
        [ActionParameter] StoreViewWithAllOptionalIdentifier storeViewIdentifier,
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

        var product = await GetProductBySkuAsync(new StoreViewOptionalIdentifier { StoreView = storeViewIdentifier.StoreView == "all" ? "default" : storeViewIdentifier.StoreView }, identifier);
        
        if(updateProductRequest.CustomAttributeKeys != null && updateProductRequest.CustomAttributeValues != null)
        {
            if(updateProductRequest.CustomAttributeKeys.Count() != updateProductRequest.CustomAttributeValues.Count())
            {
                throw new ArgumentException("Custom attribute keys and values count must be equal");
            }
        }
        
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
                custom_attributes = mergedCustomAttributes.Select(x => new
                {
                    attribute_code = x.AttributeCode,
                    value = x.Value
                }).ToList()
            }
        };
        
        var request = new ApiRequest($"/rest/{storeViewIdentifier}/V1/products/{identifier.Sku}", Method.Put, Creds)
            .AddBody(body);
        return await Client.ExecuteWithErrorHandling<ProductResponse>(request);
    }

    [Action("Update product from HTML", Description = "Update product by specified SKU with HTML content")]
    public async Task<ProductResponse> UpdateProductBySkuAsHtmlAsync(
        [ActionParameter] StoreViewWithAllOptionalIdentifier storeViewIdentifier,
        [ActionParameter] UpdateProductAsHtmlRequest updateProductAsHtmlRequest)
    {
        var htmlStream = await fileManagementClient.DownloadAsync(updateProductAsHtmlRequest.File);
        var html = await new StreamReader(htmlStream).ReadToEndAsync();
        
        var htmlModel = HtmlHelper.ConvertFromHtml(html);
        var productModel = HtmlHelper.ParseCustomAttributesFromHtml(html);
        
        var productSku = updateProductAsHtmlRequest.ProductSku ?? htmlModel.ResourceId ??
            throw new ArgumentException(
                "Couldn't find product SKU in the HTML document. Please specify it manually in the optional input.");
        var product = await GetProductBySkuAsync(new StoreViewOptionalIdentifier { StoreView = storeViewIdentifier.StoreView == "all" ? "default" : storeViewIdentifier.StoreView }, new ProductIdentifier { Sku = productSku });
        
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

        var appropriateCustomAttributes = product.CustomAttributes.Select(x =>
        {
            if (IsArrayString(x.Value))
            {
                var list = JsonConvert.DeserializeObject<List<string>>(x.Value)!
                    .ToList();
                return new CustomAttributeDto(x.AttributeCode, list);
            }
        
            return new CustomAttributeDto(x.AttributeCode, x.Value);
        }).ToList();
        
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
                custom_attributes = appropriateCustomAttributes.Select(x => new
                {
                    attribute_code = x.AttributeCode,
                    value = x.Value
                })
            }
        };

        var request = new ApiRequest($"/rest/{storeViewIdentifier}/V1/products/{productSku}", Method.Put, Creds)
            .AddBody(body);
        return await Client.ExecuteWithErrorHandling<ProductResponse>(request);
    }

    [Action("Delete product", Description = "Delete product by specified SKU")]
    public async Task DeleteProductBySkuAsync([ActionParameter] ProductIdentifier identifier)
    {
        var endpoint = $"/rest/V1/products/{identifier.Sku}";
        await Client.ExecuteWithErrorHandling(
            new ApiRequest(endpoint, Method.Delete, Creds));
    }
    
    [Action("Add custom attribute", Description = "Add custom attribute to product by specified SKU")]
    public async Task<ProductResponse> AddCustomAttributeToProductAsync(
        [ActionParameter] StoreViewOptionalIdentifier storeViewIdentifier,
        [ActionParameter] ProductIdentifier identifier,
        [ActionParameter] AddCustomAttributeRequest customAttributeDto)
    {
        var product = await GetProductBySkuAsync(storeViewIdentifier, identifier);
        product.CustomAttributes.Add(new CustomAttribute
        {
            AttributeCode = customAttributeDto.AttributeCode,
            Value = customAttributeDto.Value
        });

        var appropriateCustomAttributes = product.CustomAttributes.Select(x =>
        {
            if (IsArrayString(x.Value))
            {
                var list = JsonConvert.DeserializeObject<List<string>>(x.Value)!
                    .ToList();
                return new CustomAttributeDto(x.AttributeCode, list);
            }
        
            return new CustomAttributeDto(x.AttributeCode, x.Value);
        }).ToList();
        
        var body = new
        {
            product = new
            {
                sku = product.Sku,
                name = product.Name,
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
                custom_attributes = appropriateCustomAttributes.Select(x => new
                {
                    attribute_code = x.AttributeCode,
                    value = x.Value
                })
            }
        };

        var request = new ApiRequest($"/rest/{storeViewIdentifier}/V1/products/{identifier.Sku}", Method.Put, Creds)
            .AddBody(body);
        return await Client.ExecuteWithErrorHandling<ProductResponse>(request);
    }

    protected override string BuildQueryString(BaseFilterRequest filterRequest)
    {
        var queryString = new StringBuilder();
        if (!string.IsNullOrEmpty(filterRequest.Title))
        {
            queryString.Append($"[filterGroups][0][filters][0][field]={Uri.EscapeDataString("name")}");
            queryString.Append(
                $"&searchCriteria[filterGroups][0][filters][0][value]={Uri.EscapeDataString($"%{filterRequest.Title}%")}");
            queryString.Append(
                $"&searchCriteria[filterGroups][0][filters][0][conditionType]={Uri.EscapeDataString("like")}");
        }

        return queryString.ToString();
    }
    
    private bool IsArrayString(string value)
    {
        return value.StartsWith("[") && value.EndsWith("]");
    }
}