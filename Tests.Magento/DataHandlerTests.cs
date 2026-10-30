using Apps.Magento.DataSources;
using Apps.Magento.Models.Identifiers.Optional;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Tests.Magento.Base;

namespace Tests.Magento;

[TestClass]
public class DataHandlerTests : TestBase
{
    [TestMethod]
    public async Task StoreViewDataSource_ReturnsStoreViews()
    {
        // Arrange
        var handler = new StoreViewDataSource(InvocationContext);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);

        // Assert
        PrintJsonResult(result);
    }
    
    [TestMethod]
    public async Task ProductDataSource_ReturnsProducts()
    {
        // Arrange
        var view = new StoreViewOptionalIdentifier { StoreView = "" };
        var handler = new ProductDataSource(InvocationContext, view);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "aul" }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(result);
    }
}