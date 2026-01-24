using WADNR.EFModels.Entities;
using Microsoft.AspNetCore.Http;
using System.Linq;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Services
{
    public class UserContext
    {
        public PersonDetail User { get; set; }

        private UserContext(PersonDetail user)
        {
            User = user;
        }

        public static PersonDetail GetUserFromHttpContext(WADNRDbContext dbContext, HttpContext httpContext)
        {
            var claimsPrincipal = httpContext.User;
            if (!claimsPrincipal.Claims.Any())
            {
                return null;
            }

            // Auth0 provides email in the 'email' claim (requires 'email' scope)
            var emailClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "email");
            if (emailClaim == null)
            {
                return null;
            }

            var user = People.GetByEmailAsDetail(dbContext, emailClaim.Value);
            return user;
        }
    }
}