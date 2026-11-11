using Apps.Magento.Models.Identifiers.Optional;
using Apps.Magento.Polling;
using Apps.Magento.Polling.Models;
using Apps.Magento.Polling.Models.Requests;
using Blackbird.Applications.Sdk.Common.Polling;
using Tests.Magento.Base;

namespace Tests.Magento;

[TestClass]
public class ContentPollingTests : TestBase
{
    private ContentPollingList Polling => new(InvocationContext);
    
    [TestMethod]
    public async Task OnContentCreatedOrUpdated_IsSuccess()
    {
        // Arrange
        var memory = new DateMemory { LastInteractionDate = DateTime.UtcNow - TimeSpan.FromMinutes(10) };
        var request = new PollingEventRequest<DateMemory> { Memory = memory };
        var contentTypes = new ContentTypesOptionalIdentifier { };
        var input = new OnContentCreatedOrUpdatedRequest { };

        // Act
        var result = await Polling.OnContentCreatedOrUpdated(request, contentTypes, input);

        // Assert
        PrintJsonResult(result);
    }
}