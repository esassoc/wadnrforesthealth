using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows access to normal users and above (Normal, ProjectSteward, Admin, EsaAdmin).
/// Use for standard user operations that non-users (Unassigned) cannot perform.
/// </summary>
public class NormalUserFeature : BaseAuthorizationAttribute
{
    public NormalUserFeature() : base([RoleEnum.Normal, RoleEnum.ProjectSteward, RoleEnum.Admin, RoleEnum.EsaAdmin])
    {
    }
}
