using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Magento.Models.Requests.Products;

public class UploadProductRequest
{
    [Display("File")]
    public FileReference File { get; set; } = new();
}