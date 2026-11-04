using Apps.Magento.Constants;
using Apps.Magento.Models.Entities.Products;
using Apps.Magento.Models.Requests.Content;
using Apps.Magento.Models.Responses.Content;
using Apps.Magento.Utils;
using Blackbird.Applications.Sdk.Common.Invocation;
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
}