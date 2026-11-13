using Apps.Magento.Polling;
using Apps.Magento.Polling.Models;
using Blackbird.Applications.Sdk.Common.Polling;
using Tests.Magento.Base;

namespace Tests.Magento;

[TestClass]
public class BlockPollingTests : TestBase
{
    private BlockPollingList Polling => new(InvocationContext);
    
    [TestMethod]
    public async Task OnBlockCreated_IsSuccess()
    {
        // Arrange
        var memory = new DateMemory { LastInteractionDate = DateTime.UtcNow - TimeSpan.FromMinutes(5) };
        var request = new PollingEventRequest<DateMemory> { Memory = memory };

        // Act
        var result = await Polling.OnBlockCreated(request);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result.Result);
    }
    
    [TestMethod]
    public async Task OnBlockUpdated_IsSuccess()
    {
        // Arrange
        var memory = new DateMemory { LastInteractionDate = DateTime.UtcNow - TimeSpan.FromMinutes(5) };
        var request = new PollingEventRequest<DateMemory> { Memory = memory };

        // Act
        var result = await Polling.OnBlockUpdated(request);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result.Result);
    }
}