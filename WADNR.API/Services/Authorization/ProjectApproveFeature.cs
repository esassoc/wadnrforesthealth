using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows access to users who can approve/reject/return project proposals.
/// Includes ProjectSteward, Admin, EsaAdmin, and CanEditProgram supplemental role.
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
}
