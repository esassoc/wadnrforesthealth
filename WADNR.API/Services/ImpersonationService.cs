using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.Helpers;

namespace WADNR.API.Services;

public class ImpersonationService(IWebHostEnvironment environment, WADNRDbContext dbContext)
{
    public PersonDetail GetEffectiveUser(WADNRDbContext dbContext, PersonDetail authenticatedUser)
    {
        if (environment.IsProduction() || authenticatedUser.ImpersonatedPersonID == null)
        {
            return authenticatedUser;
        }

        var impersonatedUser = People.GetByIDAsDetail(dbContext, authenticatedUser.ImpersonatedPersonID.Value);
        return impersonatedUser ?? authenticatedUser;
    }

    public async Task<PersonDetail> ImpersonateUserAsync(HttpContext httpContext, int targetPersonID)
    {
        var globalID = httpContext.User.Claims.SingleOrDefault(c => c.Type == ClaimsConstants.Sub)?.Value;
        var originalUser = People.GetByGlobalIDAsDetail(dbContext, globalID);

        if (environment.IsProduction() || originalUser == null)
        {
            return originalUser;
        }

        var person = await dbContext.People.FindAsync(originalUser.PersonID);
        person.ImpersonatedPersonID = targetPersonID;
        await dbContext.SaveChangesWithNoAuditingAsync();

        return People.GetByIDAsDetail(dbContext, targetPersonID);
    }

    public async Task<PersonDetail> StopImpersonationAsync(HttpContext httpContext)
    {
        var globalID = httpContext.User.Claims.SingleOrDefault(c => c.Type == ClaimsConstants.Sub)?.Value;
        var originalUser = People.GetByGlobalIDAsDetail(dbContext, globalID);

        if (originalUser == null)
        {
            return originalUser;
        }

        var person = await dbContext.People.FindAsync(originalUser.PersonID);
        person.ImpersonatedPersonID = null;
        await dbContext.SaveChangesWithNoAuditingAsync();

        return People.GetByIDAsDetail(dbContext, originalUser.PersonID);
    }
}
