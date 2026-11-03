using Apps.Magento.Actions;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Requests.StoreViews;
using Tests.Magento.Base;

namespace Tests.Magento;

[TestClass]
public class StoreViewActionTests : TestBase
{
    private StoreViewActions Actions => new(InvocationContext);
    
    [TestMethod]
    public async Task SearchStoreViews_ReturnsStoreViews()
    {
        // Arrange
        var input = new SearchStoreViewsRequest { NameContains = "ish" };

        // Act
        var result = await Actions.SearchStoreViews(input);

        // Assert
        PrintJsonResult(result.StoreViews);
        Assert.IsNotNull(result.StoreViews);
    }
    
    [TestMethod]
    public async Task GetStoreView_ReturnsStoreView()
    {
        // Arrange
        var input = new StoreViewIdentifier { StoreView = "fr" };

        // Act
        var result = await Actions.GetStoreView(input);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task CreateStoreView_ReturnsCreatedStoreView()
    {
        // Arrange
        int randomCodeNumber = Random.Shared.Next(int.MaxValue);

        string code = $"bb_test_{randomCodeNumber}";
        string name = $"New store view {randomCodeNumber}";
        string locale = "uk_UA";
        string groupId = "1";
        bool isActive = true;
        
        var input = new CreateStoreViewRequest
        {
            Code = code,
            Name = name,
            Locale = locale,
            GroupId = groupId,
            IsActive = isActive
        };

        // Act
        var result = await Actions.CreateStoreView(input);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
        
        Assert.AreEqual(code, result.Code);
        Assert.AreEqual(name, result.Name);
    }
}