using Apps.Magento.Constants;
using Apps.Magento.Models.Identifiers;
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
        var memory = new ContentPollingMemory
        {
            LastInteractionDate = DateTime.UtcNow - TimeSpan.FromMinutes(5),
            ContentHashes = new Dictionary<string, string>
            {
                { "product-my test product", "22D3C61E49F3E3E5B8B6A3334B0C4F180D96776C32E6D260799D5F26510D251A" }
            },
        };
        var request = new PollingEventRequest<ContentPollingMemory> { Memory = memory };
        var contentTypes = new ContentTypesOptionalIdentifier { ContentTypes = [ContentTypeConstants.Product] };
        var input = new OnContentCreatedOrUpdatedRequest { };
        var store = new StoreViewIdentifier { StoreView = "default" };

        // Act
        var result = await Polling.OnContentCreatedOrUpdated(request, contentTypes, input, store);

        // Assert
        PrintJsonResult(result);
    }
}