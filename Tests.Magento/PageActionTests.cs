using Apps.Magento.Actions;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Requests.Pages;
using Tests.Magento.Base;

namespace Tests.Magento;

[TestClass]
public class PageActionTests : TestBase
{
    private PageActions Actions => new(InvocationContext, FileManager);
    
    [TestMethod]
    public async Task SearchPages_ReturnsPages()
    {
        // Arrange
        var input = new SearchPagesRequest { TitleContains = "" };

        // Act
        var result = await Actions.SearchPages(input);

        // Assert
        PrintJsonResult(result.Pages);
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task GetPage_ReturnsPage()
    {
        // Arrange
        var input = new PageIdentifier { PageId = "1" };

        // Act
        var result = await Actions.GetPage(input);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }
}