using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace WADNR.API.Services;

public class ArcGisAuthService(IHttpClientFactory httpClientFactory, IOptions<WADNRConfiguration> configuration)
{
    private readonly WADNRConfiguration _configuration = configuration.Value;

    /// <summary>
    /// Gets a user auth token for the Finance API endpoints (username/password flow).
    /// Used by Vendor, ProjectCode, ProgramIndex, and FundSourceExpenditure import jobs.
    /// </summary>
    public async Task<string> GetDataImportUserTokenAsync()
    {
        using var httpClient = httpClientFactory.CreateClient("FinanceApi");
        var requestBody = new[]
        {
            new KeyValuePair<string, string>("username", _configuration.DataImportAuthUsername),
            new KeyValuePair<string, string>("password", _configuration.DataImportAuthPassword),
            new KeyValuePair<string, string>("referer", "localhost"),
            new KeyValuePair<string, string>("f", "json"),
        };

        var request = new HttpRequestMessage(HttpMethod.Post, _configuration.DataImportAuthUrl)
        {
            Content = new FormUrlEncodedContent(requestBody)
        };
        request.Headers.Accept.Clear();
        request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/atom"));

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);
        var token = doc.RootElement.GetProperty("token").GetString();
        return token ?? throw new InvalidOperationException("Finance API auth response did not contain a token.");
    }

    /// <summary>
    /// Gets an application access token for GIS data import endpoints (client_credentials flow).
    /// Used by LOA, USFS, and USFS NEPA Boundary import jobs.
    /// </summary>
    public async Task<string> GetApplicationAccessTokenAsync()
    {
        using var httpClient = httpClientFactory.CreateClient("GisApi");
        var body = new Dictionary<string, string>
        {
            { "client_id", _configuration.ArcGisClientId },
            { "client_secret", _configuration.ArcGisClientSecret },
            { "grant_type", "client_credentials" }
        };

        var response = await httpClient.PostAsync(_configuration.ArcGisAuthUrl, new FormUrlEncodedContent(body));
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);
        var accessToken = doc.RootElement.GetProperty("access_token").GetString();

        if (string.IsNullOrEmpty(accessToken))
        {
            throw new InvalidOperationException("ArcGIS application auth response did not contain an access_token.");
        }

        return accessToken;
    }
}
