using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows access to users who can create or edit projects.
/// Includes Normal users, ProjectSteward, Admin, EsaAdmin, and CanEditProgram supplemental role.
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
}
