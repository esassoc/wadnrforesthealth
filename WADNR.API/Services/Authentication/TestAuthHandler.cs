using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.Models.Helpers;

namespace WADNR.API.Services.Authentication
{
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "TestScheme";
        public const string TestUserHeader = "X-E2E-User-GlobalID";

        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var headerValues = Request.Headers[TestUserHeader];
            var globalID = headerValues.Count > 0 ? headerValues[0] : null;
            if (string.IsNullOrEmpty(globalID))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var claims = new Claim[]
            {
                new(ClaimsConstants.Sub, globalID),
                new(ClaimsConstants.Emails, "e2e-test@example.com"),
            };
            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
