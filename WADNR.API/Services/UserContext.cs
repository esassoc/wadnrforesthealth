using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.Helpers;

namespace WADNR.API.Services
{
    public class UserContext
    {
        public PersonDetail User { get; set; }

        private UserContext(PersonDetail user)
        {
            User = user;
        }

        public static PersonDetail GetUserAsDetailFromHttpContext(WADNRDbContext dbContext, HttpContext httpContext)
        {
            PersonDetail user;
            var claimsPrincipal = httpContext.User;
            if (!claimsPrincipal.Claims.Any())
            {
                user = null;
            }
            else
            {
                var userGlobalID = claimsPrincipal.Claims.Single(c => c.Type == ClaimsConstants.Sub).Value;
                var user1 = People.GetByGlobalIDAsDetail(dbContext, userGlobalID);
                user = user1;
            }

            var authenticatedUser = user ?? new PersonDetail
            {
                PersonID = Person.AnonymousPersonID,
                FirstName = "Anonymous",
                LastName = "User",
                CreateDate = DateTime.UtcNow,
                LastActivityDate = DateTime.UtcNow,
                IsActive = true,
                OrganizationID = -1,
                ReceiveSupportEmails = false,
            };

            var impersonationService = httpContext.RequestServices.GetService(typeof(ImpersonationService)) as ImpersonationService;
            if (impersonationService != null)
            {
                return impersonationService.GetEffectiveUser(dbContext, authenticatedUser);
            }

            return authenticatedUser;
        }
    }
}