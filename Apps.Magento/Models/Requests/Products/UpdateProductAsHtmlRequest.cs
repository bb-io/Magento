using Apps.Magento.DataSources;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Magento.Models.Requests.Products;

public class UpdateProductAsHtmlRequest
{
    public FileReference File { get; set; } = new();

    [Display("Product SKU", Description = "The SKU of the product to update."), DataSource(typeof(ProductDataSource))]
    public string? ProductSku { get; set; }
}