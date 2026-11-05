using Apps.Magento.DataSources.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Magento.Models.Identifiers.Optional;

public class ContentTypesOptionalIdentifier
{
    [Display("Content types", Description = "Default is all content types"), StaticDataSource(typeof(ContentTypeDataSource))]
    public List<string>? ContentTypes { get; set; }
}