using Apps.Magento.Polling;
using Apps.Magento.Polling.Models;
using Blackbird.Applications.Sdk.Common.Polling;
using Tests.Magento.Base;

namespace Tests.Magento;

[TestClass]
public class PagePollingTests : TestBase
{
    private PagePollingList Polling => new(InvocationContext);

    [TestMethod]
    public async Task OnPageCreated_IsSuccess()
    {
        // Arrange
        var memory = new DateMemory { LastInteractionDate = DateTime.UtcNow - TimeSpan.FromMinutes(5) };
        var request = new PollingEventRequest<DateMemory> { Memory = memory };

        // Act
        var result = await Polling.OnPageCreated(request);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result.Result);
    }

    [TestMethod]
    public async Task OnPageUpdated_IsSuccess()
    {
        // Arrange
        var memory = new DateMemory { LastInteractionDate = DateTime.UtcNow - TimeSpan.FromMinutes(5) };
        var request = new PollingEventRequest<DateMemory> { Memory = memory };

        // Act
        var result = await Polling.OnPageUpdated(request);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result.Result);
    }
}