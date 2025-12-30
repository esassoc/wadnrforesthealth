using WADNRForestHealthTracker.EFModels.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using WADNRForestHealthTracker.Models.DataTransferObjects;

namespace WADNRForestHealthTracker.API.Services
{
    public class UserContext
    {
        public PersonSimpleDto User { get; set; }

        private UserContext(PersonSimpleDto user)
        {
            User = user;
        }

        public static PersonSimpleDto GetUserFromHttpContext(WADNRForestHealthTrackerDbContext dbContext, HttpContext httpContext)
        {

            var claimsPrincipal = httpContext.User;
            if (!claimsPrincipal.Claims.Any())
            {
                return null;
            }

            var userGuid = Guid.Parse(claimsPrincipal.Claims.Single(c => c.Type == "sub").Value);
            var keystoneUser = People.GetByEmailAsSimpleDto(dbContext, userGuid.ToString());

            return keystoneUser;
        }
    }
}