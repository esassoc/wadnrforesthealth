using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.EFModels.Entities;

namespace WADNR.Scalar.Filters;

public class ApiKeyAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    WADNRDbContext dbContext)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string ApiKeyName = "x-api-key";

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyName, out var extractedApiKey))
        {
            return AuthenticateResult.Fail("API Key was not provided");
        }

        if (!Guid.TryParse(extractedApiKey, out var parsedApiKey))
        {
            return AuthenticateResult.Fail("API Key is not valid");
        }

        var person = await dbContext.People.FirstOrDefaultAsync(p => p.ApiKey == parsedApiKey);
        if (person == null)
        {
            return AuthenticateResult.Fail("API Key is not valid");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, person.PersonID.ToString()),
            new Claim(ClaimTypes.Name, person.Email ?? string.Empty),
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}
