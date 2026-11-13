using Apps.Magento.Models.Identifiers.Optional;
using Apps.Magento.Polling;
using Apps.Magento.Polling.Models;
using Apps.Magento.Polling.Models.Requests;
using Blackbird.Applications.Sdk.Common.Polling;
using Tests.Magento.Base;

namespace Tests.Magento;

[TestClass]
public class ProductPollingTests : TestBase
{
    private ProductPollingList Polling => new(InvocationContext);

    [TestMethod]
    public async Task OnProductCreated_IsSuccess()
    {
        // Arrange
        var memory = new DateMemory { LastInteractionDate = DateTime.UtcNow - TimeSpan.FromMinutes(5) };
        var request = new PollingEventRequest<DateMemory> { Memory = memory };
        var store = new StoreViewOptionalIdentifier { };
        var input = new ProductPollingRequest
        {
        
        };

        // Act
        var result = await Polling.OnProductCreated(request, store, input);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result.Result);
    }
    
    [TestMethod]
    public async Task OnProductUpdated_IsSuccess()
    {
        // Arrange
        var memory = new DateMemory { LastInteractionDate = DateTime.UtcNow - TimeSpan.FromMinutes(5) };
        var request = new PollingEventRequest<DateMemory> { Memory = memory };
        var store = new StoreViewOptionalIdentifier { };
        var input = new ProductPollingRequest
        {
        
        };

        // Act
        var result = await Polling.OnProductUpdated(request, store, input);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result.Result);
    }
}