using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows admin-level direct editing of project data (notes, photos, documents).
/// Normal users must use the project update workflow instead.
/// Includes ProjectSteward, Admin, EsaAdmin, and CanEditProgram roles.
/// Adds entity-scoped checks: pending projects denied, steward/program scoping enforced.
/// </summary>
public class ProjectEditAsAdminFeature : BaseAuthorizationAttribute
{
    public ProjectEditAsAdminFeature() : base([
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

        // List endpoints (no projectID in route) — role check is sufficient
        if (!projectContext.HasProject || person == null) return;

        if (!ProjectAuthorization.CanEditAsAdmin(person, projectContext.AuthData!, projectContext.StewardshipAreaTypeID))
        {
            context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
        }
    }
}
