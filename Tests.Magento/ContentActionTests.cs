using Apps.Magento.Actions;
using Apps.Magento.Constants;
using Apps.Magento.Models.Identifiers.Optional;
using Apps.Magento.Models.Requests.Content;
using Tests.Magento.Base;

namespace Tests.Magento;

[TestClass]
public class ContentActionTests : TestBase
{
    private ContentActions Actions => new(InvocationContext, FileManager);

    [TestMethod]
    public async Task SearchContent_AllContentTypes_ReturnsContent()
    {
        // Arrange
        var types = new ContentTypesOptionalIdentifier { };
        var input = new SearchContentRequest { };

        // Act
        var result = await Actions.SearchContent(types, input);

        // Assert
        PrintJsonResult(result.Content);
    }

    [TestMethod]
    public async Task SearchContent_OneContentType_ReturnsContent()
    {
        // Arrange
        string contentType = ContentTypeConstants.Product;
        
        var types = new ContentTypesOptionalIdentifier { ContentTypes = [contentType] };
        var input = new SearchContentRequest { };

        // Act
        var result = await Actions.SearchContent(types, input);

        // Assert
        PrintJsonResult(result.Content);
        Assert.IsTrue(result.Content.All(x => x.ContentType == contentType));
    }
}