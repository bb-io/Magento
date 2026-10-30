using Apps.Magento.Actions;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Identifiers.Optional;
using Apps.Magento.Models.Requests.Products;
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
}