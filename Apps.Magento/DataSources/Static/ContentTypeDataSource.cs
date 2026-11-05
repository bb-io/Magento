using Apps.Magento.Constants;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Magento.DataSources.Static;

public class ContentTypeDataSource : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return ContentTypeConstants.SupportedContentTypes.Select(x => new DataSourceItem(x, char.ToUpperInvariant(x[0]) + x[1..]));
    }
}