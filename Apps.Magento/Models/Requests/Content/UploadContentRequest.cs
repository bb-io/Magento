using Apps.Magento.DataSources;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Magento.Models.Requests.Content;

public class UploadContentRequest : IUploadContentInput
{
    [Display("Content")]
    public FileReference Content { get; set; } = null!;
    
    [Display("Content ID"), DataSource(typeof(ContentDataSource))]
    public string? ContentId { get; set; } = string.Empty;

    [Display("Store view", Description = "Store view to upload the translated content to")] 
    [DataSource(typeof(StoreViewWithAllDataSource))]
    public string Locale { get; set; } = string.Empty;
}