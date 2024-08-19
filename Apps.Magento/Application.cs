using Blackbird.Applications.Sdk.Common;

namespace Apps.Magento;

public class Application : IApplication
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
}