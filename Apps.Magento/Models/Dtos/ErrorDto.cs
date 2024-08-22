using Newtonsoft.Json;

namespace Apps.Magento.Models.Dtos;

public class ErrorDto
{
    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonProperty("trace")]
    public string Trace { get; set; } = string.Empty;
}