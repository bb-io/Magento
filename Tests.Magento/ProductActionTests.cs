using Apps.Magento.Actions;
using Apps.Magento.Models.Identifiers.Optional;
using Apps.Magento.Models.Requests.Products;
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
        var input = new SearchProductsRequest { UpdatedAfter = DateTime.UtcNow.AddHours(-4) };

        // Act
        var result = await Actions.SearchProducts(storeView, input);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result.Products);
    }
}