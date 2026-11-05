using Apps.Magento.Actions;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Identifiers.Optional;
using Apps.Magento.Models.Requests.Products;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Tests.Magento.Base;

namespace Tests.Magento;

[TestClass]
public class ProductActionTests : TestBase
{
    private ProductActions Actions => new(InvocationContext, FileManager);

    [TestMethod]
    public async Task SearchProducts_ReturnsProducts()
    {
        // Arrange
        var storeView = new StoreViewOptionalIdentifier { };
        var input = new SearchProductsRequest { NameContains = "aul" };

        // Act
        var result = await Actions.SearchProducts(storeView, input);

        // Assert
        PrintJsonResult(result.Products);
        Assert.IsNotNull(result.Products);
    }
    
    [TestMethod]
    public async Task GetProduct_ReturnsProduct()
    {
        // Arrange
        var storeView = new StoreViewOptionalIdentifier { };
        var product = new ProductIdentifier { Sku = "VAULT-ZT-001" };

        // Act
        var result = await Actions.GetProduct(storeView, product);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task DownloadProduct_IsSuccess()
    {
        // Arrange
        var storeView = new StoreViewOptionalIdentifier { };
        var product = new ProductIdentifier { Sku = "VAULT-ZT-001" };
        var input = new DownloadProductRequest { CustomAttributes = ["description"] };

        // Act
        var result = await Actions.DownloadProduct(storeView, product, input);

        // Assert
        PrintJsonResult(result.File.Name);
        Assert.IsNotNull(result.File);
    }
    
    [TestMethod]
    public async Task UploadProduct_IsSuccess()
    {
        // Arrange
        var storeView = new StoreViewWithAllOptionalIdentifier { };
        var input = new UploadProductRequest { File = new FileReference { Name = "VAULT-ZT-001.html" } };
        var product = new ProductOptionalIdentifier { };

        // Act
        await Actions.UploadProduct(storeView, input, product);
    }

    [TestMethod]
    public async Task CreateProduct_ReturnsCreatedProduct()
    {
        // Arrange
        string sku = Guid.NewGuid().ToString();
        string productName = $"testProductName_{sku}";
        string attributeSetId = "4";
        double price = 99.99;
        string typeId = "downloadable";
        double weight = 1.23;
        
        var input = new CreateProductRequest
        {
            Sku = sku,
            Name = productName,
            AttributeSetId = attributeSetId,
            Price = price,
            Type = typeId,
            Weight = weight
        };

        // Act
        var result = await Actions.CreateProduct(input);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
        
        Assert.AreEqual(sku, result.Sku);
        Assert.AreEqual(productName, result.Name);
        Assert.AreEqual(attributeSetId, result.AttributeSetId);
        Assert.AreEqual(price, result.Price);
        Assert.AreEqual(typeId, result.TypeId);
        Assert.AreEqual(weight, result.Weight);
    }
    
    [TestMethod]
    public async Task UpdateProduct_ReturnsUpdatedProduct()
    {
        // Arrange
        string productName = $"testProductName_{Guid.NewGuid()}";
        string attributeSetId = "4";
        double price = 999.99;
        string typeId = "downloadable";
        double weight = 91.23;

        var storeView = new StoreViewWithAllOptionalIdentifier { };
        var product = new ProductIdentifier { Sku = "9d4d2e77-18b0-45bd-85b5-c32854df928e" };
        var input = new UpdateProductRequest
        {
            Name = productName,
            AttributeSetId = attributeSetId,
            Price = price,
            Weight = weight
        };

        // Act
        var result = await Actions.UpdateProduct(storeView, product, input);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
        
        Assert.AreEqual(productName, result.Name);
        Assert.AreEqual(attributeSetId, result.AttributeSetId);
        Assert.AreEqual(price, result.Price);
        Assert.AreEqual(typeId, result.TypeId);
        Assert.AreEqual(weight, result.Weight);
    }

    [TestMethod]
    public async Task DeleteProduct_IsSuccess()
    {
        // Arrange
        var identifier = new ProductIdentifier { Sku = "6b31bbc2-4f02-4a31-b426-f3d3120cab61" };

        // Act
        await Actions.DeleteProduct(identifier);
    }

    [TestMethod]
    public async Task AddCustomAttributeToProduct_ReturnsUpdatedProduct()
    {
        // Arrange
        string attrKey = "description";
        string attrValue = Guid.NewGuid().ToString();
        
        var storeView = new StoreViewOptionalIdentifier { };
        var product = new ProductIdentifier { Sku = "9d4d2e77-18b0-45bd-85b5-c32854df928e" };
        var input = new AddCustomAttributeRequest
        {
            AttributeCode = attrKey,
            Value = attrValue
        };

        // Act
        var result = await Actions.AddCustomAttributeToProduct(storeView, product, input);

        // Assert
        PrintJsonResult(result);

        var updatedAttribute = result.CustomAttributes.FirstOrDefault(x => x.AttributeCode == attrKey);
        Assert.IsNotNull(updatedAttribute);
        Assert.AreEqual(attrValue, updatedAttribute.Value);
    }
}