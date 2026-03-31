using System.Text;
using System.Text.Json;
using WADNR.API.Services.Authentication;

namespace WADNR.API.Tests.Helpers;

public static class HttpResponseHelper
{
    /// <summary>
    /// Sends an authenticated GET request as a specific user (identified by GlobalID).
    /// Uses the UnauthenticatedHttpClient with a per-request auth header.
    /// </summary>
    public static async Task<HttpResponseMessage> GetAsUserAsync(string route, string globalID)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, route);
        request.Headers.Add(TestAuthHandler.TestUserHeader, globalID);
        return await AssemblySteps.UnauthenticatedHttpClient.SendAsync(request);
    }

    /// <summary>
    /// Sends an authenticated POST request (with JSON body) as a specific user.
    /// </summary>
    public static async Task<HttpResponseMessage> PostAsUserAsync<T>(string route, string globalID, T body)
    {
        var json = JsonSerializer.Serialize(body, AssemblySteps.DefaultJsonSerializerOptions);
        var request = new HttpRequestMessage(HttpMethod.Post, route)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        request.Headers.Add(TestAuthHandler.TestUserHeader, globalID);
        return await AssemblySteps.UnauthenticatedHttpClient.SendAsync(request);
    }

    /// <summary>
    /// Sends an authenticated PUT request (with JSON body) as a specific user.
    /// </summary>
    public static async Task<HttpResponseMessage> PutAsUserAsync<T>(string route, string globalID, T body)
    {
        var json = JsonSerializer.Serialize(body, AssemblySteps.DefaultJsonSerializerOptions);
        var request = new HttpRequestMessage(HttpMethod.Put, route)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        request.Headers.Add(TestAuthHandler.TestUserHeader, globalID);
        return await AssemblySteps.UnauthenticatedHttpClient.SendAsync(request);
    }

    public static async Task<HttpResponseMessage> PostAsJsonAsync<T>(
        this HttpClient client, string route, T body)
    {
        var json = JsonSerializer.Serialize(body, AssemblySteps.DefaultJsonSerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await client.PostAsync(route, content);
    }

    public static async Task<HttpResponseMessage> PutAsJsonAsync<T>(
        this HttpClient client, string route, T body)
    {
        var json = JsonSerializer.Serialize(body, AssemblySteps.DefaultJsonSerializerOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await client.PutAsync(route, content);
    }

    public static async Task<T> DeserializeContentAsync<T>(this HttpResponseMessage response)
    {
        var contentString = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(contentString))
        {
            return default!;
        }

        return JsonSerializer.Deserialize<T>(contentString, AssemblySteps.DefaultJsonSerializerOptions)!;
    }

    public static async Task<T?> DeserializeIfSuccessAsync<T>(this HttpResponseMessage response) where T : class
    {
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var contentString = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(contentString))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(contentString, AssemblySteps.DefaultJsonSerializerOptions);
    }
}
