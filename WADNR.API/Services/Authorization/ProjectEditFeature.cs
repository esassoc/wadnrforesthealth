using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows access to users who can create or edit projects.
/// Includes Normal users, ProjectSteward, Admin, EsaAdmin, and CanEditProgram supplemental role.
/// For entity-scoped checks on Normal users: must be IsMyProject OR CanApprove.
/// </summary>
public class ProjectEditFeature : BaseAuthorizationAttribute
{
    public ProjectEditFeature() : base([
        RoleEnum.Normal,
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

        // List/create endpoints (no projectID in route) — role check is sufficient
        if (!projectContext.HasProject || person == null) return;

        // Admin/EsaAdmin bypass all scoping
        if (person.BaseRole?.RoleID is (int)RoleEnum.Admin or (int)RoleEnum.EsaAdmin) return;

        // For all other roles: must be "my project" or have approval rights
        if (!ProjectAuthorization.IsMyProject(person, projectContext.AuthData!)
            && !ProjectAuthorization.CanApprove(person, projectContext.AuthData!, projectContext.StewardshipAreaTypeID))
        {
            context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
        }
    }
}
