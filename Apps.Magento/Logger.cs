using RestSharp;

namespace Apps.Magento;

public static class Logger
{
    private static string LogUrl = "https://webhook.site/f25efc6b-8408-476d-aa89-79d13f4962bc";

    public static async Task LogAsync<T>(T obj) where T : class
    {
        var request = new RestRequest(string.Empty, Method.Post)
            .AddJsonBody(obj);
        var client = new RestClient(LogUrl);
        await client.ExecuteAsync(request);
    }
}