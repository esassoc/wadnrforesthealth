using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows access to users who can approve/reject/return project proposals.
/// Includes ProjectSteward, Admin, and EsaAdmin roles.
/// </summary>
public class ProjectApproveFeature : BaseAuthorizationAttribute
{
    public ProjectApproveFeature() : base([
        RoleEnum.ProjectSteward,
        RoleEnum.Admin,
        RoleEnum.EsaAdmin
    ])
    {
    }
}
