using Apps.Magento.Invocables;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Magento.Services;

public abstract class BaseContentService(InvocationContext invocationContext) : AppInvocable(invocationContext)
{
    protected abstract string ContentType { get; }
}