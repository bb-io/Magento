using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;

namespace Apps.Magento;

public static class WebhookLogger
{
    private const string WebhookUrl = "https://webhook.site/10569094-e5ef-4839-9e81-42fd4ecccde9";

    public static async Task LogAsync<T>(T obj) where T : class
    {
        var restClient = new RestClient(WebhookUrl);
        var restRequest = new RestRequest(string.Empty, Method.Post)
            .WithJsonBody(obj);
        
        await restClient.ExecuteAsync(restRequest);
    }
    
    public static void Log<T>(T obj) where T : class
    {
        LogAsync(obj).Wait();
    }
    
    public static async Task LogAsync(Exception exception)
    {
        await LogAsync(new
        {
            Exception = exception.Message,
            exception.StackTrace,
            InnerException = exception.InnerException?.Message,
            ExceptionType = exception.GetType().Name
        });
    }
}