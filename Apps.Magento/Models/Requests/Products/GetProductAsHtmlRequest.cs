using Apps.Magento.DataSources;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Magento.Models.Requests.Products;

public class GetProductAsHtmlRequest
{
    [Display("Custom attributes", Description = "Specify custom attributes to include in the HTML file. By default, only name, meta_title, meta_description, meta_keyword and description are included."), DataSource(typeof(ProductAttributeDataSource))]
    public IEnumerable<string>? CustomAttributes { get; set; }
}