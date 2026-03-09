using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.Helpers;

namespace WADNR.API.Services.Authorization;

public class StopImpersonationFeature : BaseAuthorizationAttribute
{
    public StopImpersonationFeature() : base([])
    {
    }

    protected override void OnAuthorizationCore(AuthorizationFilterContext context, WADNRDbContext dbContext, PersonDetail? person)
    {
        var globalID = context.HttpContext.User.Claims
            .SingleOrDefault(c => c.Type == ClaimsConstants.Sub)?.Value;
        var originalUser = People.GetByGlobalIDAsDetail(dbContext, globalID);

        var isAdmin = originalUser?.BaseRole != null &&
            (originalUser.BaseRole.RoleID == (int)RoleEnum.Admin ||
             originalUser.BaseRole.RoleID == (int)RoleEnum.EsaAdmin);
        var isImpersonating = originalUser?.ImpersonatedPersonID != null;

        if (!isAdmin || !isImpersonating)
        {
            context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
        }
    }
}
