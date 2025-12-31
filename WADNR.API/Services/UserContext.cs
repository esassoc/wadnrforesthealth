using WADNR.EFModels.Entities;
using Microsoft.AspNetCore.Http;
using System;
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

            var userGuid = Guid.Parse(claimsPrincipal.Claims.Single(c => c.Type == "sub").Value);
            var keystoneUser = People.GetByEmailAsDetail(dbContext, userGuid.ToString());

            return keystoneUser;
        }
    }
}