using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Metadata;

namespace Apps.Magento;

public class Application : IApplication, ICategoryProvider
{
    public string Name
    {
        get => "Magento";
        set { }
    }

    public T GetInstance<T>()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ApplicationCategory> Categories
    {
        get => new [] { ApplicationCategory.ECommerce };
        set { }
    }
}