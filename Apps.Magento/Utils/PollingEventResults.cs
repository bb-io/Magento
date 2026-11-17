using Apps.Magento.Polling.Models;
using Blackbird.Applications.Sdk.Common.Polling;

namespace Apps.Magento.Utils;

public static class PollingEventResults
{
    // Timestamps have second precision. Going back one second makes the next poll recheck the boundary second instead of missing it
    public static DateTime PollingStart => DateTime.UtcNow.AddSeconds(-1);
    
    public static PollingEventResponse<DateMemory, T> NoFlight<T>(DateTime pollingStart)
    {
        return new()
        {
            FlyBird = false,
            Memory = new() { LastInteractionDate = pollingStart }
        };
    }
    
    public static PollingEventResponse<TMemory, TResult> NoFlight<TMemory, TResult>(TMemory memory)
    {
        return new()
        {
            FlyBird = false,
            Memory = memory,
        };
    }
    
    public static PollingEventResponse<DateMemory, T> Flight<T>(T result, DateTime pollingStart)
    {
        return new()
        {
            Result = result,
            FlyBird = true,
            Memory = new() { LastInteractionDate = pollingStart }
        };
    }
}