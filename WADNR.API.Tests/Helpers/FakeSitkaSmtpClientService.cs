using System.Net.Http.Headers;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using WADNR.Common.EMail;

namespace WADNR.API.Tests.Helpers;

public class FakeSitkaSmtpClientService : SitkaSmtpClientService
{
    public FakeSitkaSmtpClientService()
        : base(new FakeSendGridClient(), Options.Create(new SendGridConfiguration()))
    {
    }

    public new Task Send(MailMessage message)
    {
        return Task.CompletedTask;
    }

    public new Task SendDirectly(MailMessage mailMessage)
    {
        return Task.CompletedTask;
    }

    private class FakeSendGridClient : ISendGridClient
    {
        private readonly HttpClient _httpClient = new();
        public string UrlPath { get; set; } = string.Empty;
        public string Version { get; set; } = "v3";
        public string MediaType { get; set; } = "application/json";

        public AuthenticationHeaderValue AddAuthorization(KeyValuePair<string, string> header)
        {
            return new AuthenticationHeaderValue("Bearer", "fake");
        }

        public Task<Response> MakeRequest(HttpRequestMessage request, CancellationToken cancellationToken = default)
            => Task.FromResult(new Response(System.Net.HttpStatusCode.OK, null, null));

        public Task<Response> RequestAsync(SendGridClient.Method method, string? requestBody = null,
            string? queryParams = null, string? requestUri = null,
            CancellationToken cancellationToken = default)
            => Task.FromResult(new Response(System.Net.HttpStatusCode.OK, null, null));

        public Task<Response> SendEmailAsync(SendGridMessage msg, CancellationToken cancellationToken = default)
            => Task.FromResult(new Response(System.Net.HttpStatusCode.OK, null, null));
    }
}
