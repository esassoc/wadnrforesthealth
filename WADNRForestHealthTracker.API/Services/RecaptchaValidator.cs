using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace WADNRForestHealthTracker.API.Services
{
    public class RecaptchaValidator
    {
        public RecaptchaValidator()
        {
        }

        public static async Task<bool> IsValidResponseAsync(string response, string secret, string verifyURL, double scoreThreshold)
        {
            var parameters = new Dictionary<string, string> { { "secret", secret }, { "response", response } };
            var encodedContent = new FormUrlEncodedContent(parameters);

            HttpClient httpClient = new HttpClient();
            var httpResponse = await httpClient.PostAsync(verifyURL, encodedContent);
            if (httpResponse.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }

            var recaptchaResponseJson = await httpResponse.Content.ReadFromJsonAsync<GoogleRecaptchaV3Response>();

            var score = recaptchaResponseJson.Score;
            switch (recaptchaResponseJson.Success)
            {
                case true:
                    return score > scoreThreshold;
                case false:
                    return false;
            }
        }
    }

    public class GoogleRecaptchaV3Response
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        [JsonPropertyName("challenge_ts")]
        public DateTime ChallengeTimestamp { get; set; }
        [JsonPropertyName("hostname")]
        public string HostName { get; set; }
        [JsonPropertyName("score")]
        public double Score { get; set; }
        [JsonPropertyName("action")]
        public string Action { get; set; }
    }
}
