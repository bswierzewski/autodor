using System.Text;
using System.Text.Json;

namespace Autodor.Tests.E2E.Core.Extensions;

public static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage> PostJsonAsync<T>(this HttpClient client, string endpoint, T data)
    {
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await client.PostAsync(endpoint, content);
    }
}