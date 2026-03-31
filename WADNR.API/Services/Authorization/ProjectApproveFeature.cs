using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows access to users who can approve/reject/return project proposals.
/// Includes ProjectSteward, Admin, EsaAdmin, and CanEditProgram supplemental role.
/// Adds entity-scoped checks: steward/program scoping enforced.
/// </summary>
public class ProjectApproveFeature : BaseAuthorizationAttribute
{
    public ProjectApproveFeature() : base([
        RoleEnum.ProjectSteward,
        RoleEnum.Admin,
        RoleEnum.EsaAdmin,
        RoleEnum.CanEditProgram
    ])
    {
    }

    protected override void OnAuthorizationCore(AuthorizationFilterContext context, WADNRDbContext dbContext, PersonDetail? person)
    {
        var projectContext = context.HttpContext.RequestServices
            .GetRequiredService<ProjectAuthorizationContext>();

        if (!projectContext.HasProject || person == null) return;

        if (!ProjectAuthorization.CanApprove(person, projectContext.AuthData!, projectContext.StewardshipAreaTypeID))
        {
            context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
        }
    }
}
