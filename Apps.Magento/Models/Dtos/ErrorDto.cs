using Newtonsoft.Json;

namespace Apps.Magento.Models.Dtos;

public class ErrorDto
{
    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;

    [JsonProperty("parameters")]
    public ParametersDto? Parameters { get; set; } 
    
    [JsonProperty("trace")]
    public string Trace { get; set; } = string.Empty;
    
    public override string ToString()
    {
        var errorMessage = $"Error message: {Message}";
        
        if (Parameters != null)
        {
            var parametersAdded = false;
            if (!string.IsNullOrEmpty(Parameters.FieldName) && errorMessage.Contains("%field"))
            {
                errorMessage = errorMessage.Replace("%field", Parameters.FieldName);
                parametersAdded = true;
            }
            
            if (!string.IsNullOrEmpty(Parameters.Value) && errorMessage.Contains("%value"))
            {
                errorMessage = errorMessage.Replace("%value", Parameters.Value);
                parametersAdded = true;
            }

            if (!parametersAdded)
            {
                errorMessage += $", Parameters: {Parameters}";
            }
        }
        
        return errorMessage;
    }
}

public class ParametersDto
{
    [JsonProperty("fieldName")]
    public string FieldName { get; set; } = string.Empty;

    [JsonProperty("value")]
    public string Value { get; set; } = string.Empty;
    
    public override string ToString()
    {
        return $"Field: {FieldName}, Value: {Value}";
    }
}