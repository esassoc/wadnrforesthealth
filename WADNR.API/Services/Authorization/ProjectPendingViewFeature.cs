using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows access to pending project lists. Only authenticated users (not Unassigned)
/// can view pending projects. Admins/ProjectStewards see all; Normal users see only their org's.
/// CanEditProgram users must have program overlap.
/// The actual per-project visibility is also enforced in the static helpers.
/// </summary>
public class ProjectPendingViewFeature : BaseAuthorizationAttribute
{
    public ProjectPendingViewFeature() : base([
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

        // List endpoints (no projectID in route) — role check is sufficient, per-row visibility in helpers
        if (!projectContext.HasProject || person == null) return;

        // Admin/EsaAdmin/ProjectSteward can view all pending projects
        if (person.HasElevatedProjectAccess()) return;

        // CanEditProgram users need program overlap
        if (person.HasCanEditProgramRole()
            && !ProjectAuthorization.CanProgramEditorManageProject(person, projectContext.AuthData!))
        {
            context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
            return;
        }

        // Normal users can only view pending projects from their org
        if (!person.HasCanEditProgramRole()
            && !ProjectAuthorization.IsMyProject(person, projectContext.AuthData!))
        {
            context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
        }
    }
}
