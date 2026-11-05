using Apps.Magento.Actions;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Identifiers.Optional;
using Apps.Magento.Models.Requests.Pages;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
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
    
    [TestMethod]
    public async Task DownloadPage_IsSuccess()
    {
        // Arrange
        var input = new PageIdentifier { PageId = "1" };

        // Act
        var result = await Actions.DownloadPage(input);

        // Assert
        Console.WriteLine(result.File.Name);
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task CreatePage_ReturnsCreatedPage()
    {
        // Arrange
        string identifier = Guid.NewGuid().ToString();
        string title = $"newPage_{identifier}";
        
        var storeView = new StoreViewWithAllOptionalIdentifier { };
        var input = new CreatePageRequest
        {
            Identifier = identifier,
            Title = title,
            File = new FileReference { Name = "test.html" },
        };

        // Act
        var result = await Actions.CreatePage(storeView, input);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
        
        Assert.AreEqual(identifier, result.Identifier);
        Assert.AreEqual(title, result.Title);
    }
    
    [TestMethod]
    public async Task UploadPage_IsSuccess()
    {
        // Arrange
        var page = new PageOptionalIdentifier { PageId = "6" };
        var input = new UploadPageRequest { File = new FileReference { Name = "test.html" } };

        // Act
        await Actions.UploadPage(page, input);
    }
    
    [TestMethod]
    public async Task UpdatePage_ReturnsUpdatedPage()
    {
        // Arrange
        string identifier = Guid.NewGuid().ToString();
        string title = $"updatedPage_{identifier}";
        string metaTitle = $"metaTitle_{title}";
        string metaKeywords = $"metaKeywords_{title}";
        string metaDescription = $"metaDesc_{title}";
        string contentHeading = $"contentHeading_{title}";
        bool active = true;
        
        var page = new PageIdentifier { PageId = "6" };
        var input = new UpdatePageRequest
        {
            Identifier = identifier,
            Title = title,
            MetaTitle = metaTitle,
            MetaDescription = metaDescription,
            MetaKeywords = metaKeywords,
            ContentHeading = contentHeading,
            Active = active
        };

        // Act
        var result = await Actions.UpdatePage(page, input);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
        
        Assert.AreEqual(identifier, result.Identifier);
        Assert.AreEqual(title, result.Title);
        Assert.AreEqual(metaTitle, result.MetaTitle);
        Assert.AreEqual(metaKeywords, result.MetaKeywords);
        Assert.AreEqual(metaDescription, result.MetaDescription);
        Assert.AreEqual(contentHeading, result.ContentHeading);
        Assert.AreEqual(active, result.Active);
        Assert.IsGreaterThan(DateTime.MinValue, result.UpdatedAt);
        Assert.IsGreaterThan(DateTime.MinValue, result.CreatedAt);
    }
    
    [TestMethod]
    public async Task DeletePage_IsSuccess()
    {
        // Arrange
        var page = new PageIdentifier { PageId = "7" };

        // Act
        await Actions.DeletePage(page);

        // Assert
        await Assert.ThrowsAsync<PluginApplicationException>(() => Actions.GetPage(page));
    }
}