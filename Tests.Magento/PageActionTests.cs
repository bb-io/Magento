using Apps.Magento.Actions;
using Apps.Magento.Models.Requests.Pages;
using Tests.Magento.Base;

namespace Tests.Magento;

[TestClass]
public class PageActionTests : TestBase
{
    private PageActions Actions => new(InvocationContext, FileManager);
    
    [TestMethod]
    public async Task GetAllPagesAsync_IsSuccess()
    {
        // Arrange
        var input = new FilterPageRequest { Title = "found" };

        // Act
        var result = await Actions.GetAllPagesAsync(input);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result.Items);
    }
}