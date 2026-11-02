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
    
    [TestMethod]
    public async Task ProductAttributeSetDataSource_ReturnsProductAttributeSets()
    {
        // Arrange
        var handler = new ProductAttributeSetDataSource(InvocationContext);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(result);
    }
    
    [TestMethod]
    public async Task ProductTypeDataSource_ReturnsProductTypes()
    {
        // Arrange
        var handler = new ProductTypeDataSource(InvocationContext);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(result);
    }
    
    [TestMethod]
    public async Task ProductAttributeDataSource_ReturnsProductAttribute()
    {
        // Arrange
        var handler = new ProductAttributeDataSource(InvocationContext);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "desc" }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(result);
    }
}