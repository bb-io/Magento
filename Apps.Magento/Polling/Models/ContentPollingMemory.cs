namespace Apps.Magento.Polling.Models;

public class ContentPollingMemory
{
    public DateTime LastInteractionDate { get; set; }
    public Dictionary<string, string> ContentHashes { get; set; } = [];
}