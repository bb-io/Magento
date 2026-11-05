using Apps.Magento.DataSources;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Magento.Models.Requests.Products;

public class DownloadProductRequest
{
    [Display("Custom attributes",
         Description = "Custom attributes to include in the file. " +
                       "By default, only name, meta_title, meta_description, meta_keyword and description are included.")]
    [DataSource(typeof(ProductAttributeDataSource))]
    public IEnumerable<string>? CustomAttributes { get; set; }
}