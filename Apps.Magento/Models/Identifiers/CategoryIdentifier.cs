using Apps.Magento.DataSources;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Magento.Models.Identifiers;

public class CategoryIdentifier
{
    [Display("Category ID", Description = "Unique identifier of the category"), DataSource(typeof(CategoryDataHandler))]
    public string CategoryId { get; set; } = string.Empty;
    
    public override string ToString()
    {
        return CategoryId;
    }
}