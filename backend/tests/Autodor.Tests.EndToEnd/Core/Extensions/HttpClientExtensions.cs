using System.Text;
using System.Text.Json;

namespace Autodor.Tests.E2E.Core.Extensions;

/// <summary>
/// Extensions for HttpClient to simplify common test operations.
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    /// Posts an object as JSON to the specified endpoint.
    /// </summary>
    /// <typeparam name="T">The type of data to serialize and post.</typeparam>
    /// <param name="client">The HTTP client instance.</param>
    /// <param name="endpoint">The API endpoint to post to.</param>
    /// <param name="data">The data to serialize as JSON and send.</param>
    /// <returns>The HTTP response from the server.</returns>
    public static async Task<HttpResponseMessage> PostJsonAsync<T>(this HttpClient client, string endpoint, T data)
    {
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await client.PostAsync(endpoint, content);
    }
}