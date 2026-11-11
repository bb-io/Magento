using Apps.Magento.Constants;
using Apps.Magento.Extensions;
using Apps.Magento.Models.Dtos;
using Apps.Magento.Models.Entities.Products;
using Apps.Magento.Models.Entities.StoreViews;
using Apps.Magento.Models.Requests.Content;
using Apps.Magento.Models.Responses.Content;
using Apps.Magento.Models.Utility.Files;
using Apps.Magento.Services.Models;
using Apps.Magento.Utils;
using Apps.Magento.Utils.Converters;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;

namespace Apps.Magento.Services.Concrete;

public class ProductService(InvocationContext invocationContext) : BaseContentService(invocationContext), IContentService
{
    protected override string ContentType => ContentTypeConstants.Product;
    
    public async Task<List<ContentResponse>> SearchContent(SearchContentRequest input)
    {
        RestRequest request = new SearchCriteriaBuilder()
            .WhereContains(("name", input.NameContains))
            .WhereLessThan(("created_at", input.CreatedBefore))
            .WhereGreaterThan(("created_at", input.CreatedAfter))
            .WhereLessThan(("updated_at", input.UpdatedBefore))
            .WhereGreaterThan(("updated_at", input.UpdatedAfter))
            .ApplyTo(new RestRequest($"{input.StoreView}/V1/products"));
        
        var products = await Client.Paginate<ProductEntity>(request);

        if (input.CategoryIds != null)
            products = products.Where(x => x.IsInAllCategories(input.CategoryIds));

        return products.Select(x => new ContentResponse(x.Sku, ContentType, x.Name, x.CreatedAt, x.UpdatedAt)).ToList();
    }

    public async Task<DownloadedContent> DownloadContent(DownloadContentServiceRequest input)
    {
        string contentId = input.ContentId;
        
        var storeView = StoreViewHelper.ForRead(input.StoreView);
        var product = await Client.GetProduct(contentId, storeView);
        var storeConfig = await Client.GetStoreConfig(storeView);
        
        var html = ProductConverter.ToHtml(product, input.CustomAttributes?.ToList());
        var metadata = new ContentMetadata(
            ContentType,
            contentId,
            storeConfig.Language,
            new Ucid(ContentType, contentId),
            product.Name,
            GetPublicUrl(product, storeConfig));
        var htmlStream = HtmlHelper.ConvertToHtml(metadata, html);
        
        return new(htmlStream, "text/html", product.Name.ToHtmlFilename(product.Sku));
    }

    public async Task<UploadedContent> UploadContent(UploadContentServiceRequest input)
    {
        var productModel = ProductConverter.FromHtml(input.Html);
        string sku = input.ContentId;

        var product = await Client.GetProduct(sku, StoreViewHelper.ForRead(input.StoreView));

        var body = new
        {
            product = new
            {
                sku,
                name = string.IsNullOrWhiteSpace(productModel.Name) ? product.Name : productModel.Name,
                custom_attributes = productModel.CustomAttributes.Select(x => new CustomAttributeDto(x.AttributeCode, x.Value))
            }
        };

        string endpoint = $"{StoreViewHelper.ForWrite(input.StoreView)}/V1/products/{Uri.EscapeDataString(sku)}";
        var request = new RestRequest(endpoint, Method.Put).WithJsonBody(body, JsonConfig.JsonSettings);

        var updated = await Client.ExecuteWithErrorHandling<ProductEntity>(request);
        return new(new Ucid(ContentType, updated.Sku), updated.Name);
    }
    
    private static string? GetPublicUrl(ProductEntity product, StoreConfigEntity storeConfig)
    {
        const int notVisibleIndividually = 1;
        var urlKey = product.GetAttributeValue("url_key");

        return product.Visibility == notVisibleIndividually || urlKey is null
            ? null
            : $"{storeConfig.BaseLinkUrl}{urlKey}.html";
    }
}