using Apps.Magento.Actions;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Identifiers.Optional;
using Apps.Magento.Models.Requests.Blocks;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Tests.Magento.Base;

namespace Tests.Magento;

[TestClass]
public class BlockActionTests : TestBase
{
    private BlockActions Actions => new(InvocationContext, FileManager);
    
    [TestMethod]
    public async Task SearchBlocks_ReturnsBlocks()
    {
        // Arrange
        var input = new SearchBlocksRequest
        {
            CreatedBefore = DateTime.UtcNow
        };

        // Act
        var result = await Actions.SearchBlocks(input);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task GetBlock_ReturnsBlock()
    {
        // Arrange
        var identifier = new BlockIdentifier { BlockId = "1" };

        // Act
        var result = await Actions.GetBlock(identifier);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task DownloadBlock_IsSuccess()
    {
        // Arrange
        var identifier = new BlockIdentifier { BlockId = "1" };

        // Act
        var result = await Actions.DownloadBlock(identifier);

        // Assert
        Console.WriteLine(result.File.Name);
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task UploadBlock_IsSuccess()
    {
        // Arrange
        var input = new UploadBlockRequest { File = new FileReference { Name = "test.html" } };
        var identifier = new BlockOptionalIdentifier { };
        var store = new StoreViewWithAllOptionalIdentifier { };

        // Act
        await Actions.UploadBlock(input, identifier, store);
    }
    
    [TestMethod]
    public async Task UpdateBlock_EmptyInputs_ThrowsMisconfigException()
    {
        // Arrange
        var block = new BlockIdentifier { BlockId = "1" };
        var input = new UpdateBlockRequest { };

        // Act
        await Assert.ThrowsAsync<PluginMisconfigurationException>(() => Actions.UpdateBlock(block, input));
    }

    [TestMethod]
    public async Task UpdateBlock_WithInputs_ReturnsUpdatedBlock()
    {
        // Arrange
        string newIdentifier = Guid.NewGuid().ToString();
        string newTitle = $"updatedBlock_{newIdentifier}";
        bool active = true;
        
        var block = new BlockIdentifier { BlockId = "1" };
        var input = new UpdateBlockRequest
        {
            Identifier = newIdentifier,
            Title = newTitle,
            Active = active
        };

        // Act
        var result = await Actions.UpdateBlock(block, input);
        
        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
        
        Assert.AreEqual(newIdentifier, result.Identifier);
        Assert.AreEqual(newTitle, result.Title);
        Assert.AreEqual(active, result.Active);
    }
    
    [TestMethod]
    public async Task CreateBlock_ReturnsCreatedBlock()
    {
        // Arrange
        string identifier = Guid.NewGuid().ToString();
        string title = $"newBlock_{identifier}";
        
        var view = new StoreViewWithAllOptionalIdentifier { StoreView = "all" };
        var input = new CreateBlockRequest
        {
            Identifier = identifier,
            Title = title,
            File = new FileReference { Name = "test.html" }
        };

        // Act
        var result = await Actions.CreateBlock(view, input);
        
        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
        
        Assert.AreEqual(identifier, result.Identifier);
        Assert.AreEqual(title, result.Title);
    }
    
    [TestMethod]
    public async Task DeleteBlock_IsSuccess()
    {
        // Arrange
        var identifier = new BlockIdentifier { BlockId = "5" };
        
        // Act
        await Actions.DeleteBlock(identifier);
        
        // Assert
        await Assert.ThrowsAsync<PluginApplicationException>(() => Actions.GetBlock(identifier));
    }
}