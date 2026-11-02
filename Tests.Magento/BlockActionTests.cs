using Apps.Magento.Actions;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Requests.Blocks;
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
}