using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows access to users who can edit GIS import configuration and download project GDBs.
/// Includes Admin, EsaAdmin, and CanEditProgram supplemental role.
/// CanEditProgram users must be assigned to the specific program.
/// Matches legacy ProgramEditMappingsFeature / ProgramEditorFeature.
/// </summary>
public class ProgramEditMappingsFeature : BaseAuthorizationAttribute
{
    public ProgramEditMappingsFeature() : base([
        RoleEnum.Admin,
        RoleEnum.EsaAdmin,
        RoleEnum.CanEditProgram
    ])
    {
    }

    protected override void OnAuthorizationCore(AuthorizationFilterContext context, WADNRDbContext dbContext, PersonDetail? person)
    {
        var programContext = context.HttpContext.RequestServices
            .GetRequiredService<ProgramAuthorizationContext>();

        // List endpoints (no programID in route) — role check is sufficient
        if (!programContext.HasProgram || person == null) return;

        if (!ProgramAuthorization.CanEditProgram(person, programContext.ProgramID!.Value))
        {
            context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
        }
    }
}
