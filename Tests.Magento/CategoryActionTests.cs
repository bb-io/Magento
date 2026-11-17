using Apps.Magento.Actions;
using Apps.Magento.Models.Identifiers;
using Apps.Magento.Models.Requests.Categories;
using Tests.Magento.Base;

namespace Tests.Magento;

[TestClass]
public class CategoryActionTests : TestBase
{
    private CategoryActions Actions => new(InvocationContext);

    [TestMethod]
    public async Task SearchCategories_ReturnsCategories()
    {
        // Arrange
        var input = new SearchCategoriesRequest
        {
            CreatedBefore = new DateTime(2026, 09, 26, 19, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var result = await Actions.SearchCategories(input);

        // Assert
        PrintJsonResult(result.Categories);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task GetCategory_ReturnsCategory()
    {
        // Arrange
        var category = new CategoryIdentifier { CategoryId = "1" };

        // Act
        var result = await Actions.GetCategory(category);

        // Assert
        PrintJsonResult(result);
        Assert.IsNotNull(result);
    }
}