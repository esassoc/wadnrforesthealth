using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows access to pending project lists. Only authenticated users (not Unassigned)
/// can view pending projects. Admins/ProjectStewards see all; Normal users see only their org's.
/// The actual per-project visibility is enforced in the static helpers.
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
}
