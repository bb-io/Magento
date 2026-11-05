using Apps.Magento.DataSources.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Magento.Models.Identifiers;

public class ContentTypeIdentifier
{
    [Display("Content type"), StaticDataSource(typeof(ContentTypeDataSource))]
    public string ContentType { get; set; } = string.Empty;
}