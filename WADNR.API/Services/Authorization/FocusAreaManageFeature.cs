using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows access to users who can manage focus area locations.
/// Includes ProjectSteward, Admin, and EsaAdmin roles.
/// Matches legacy FocusAreaManageFeature.
/// </summary>
public class FocusAreaManageFeature : BaseAuthorizationAttribute
{
    public FocusAreaManageFeature() : base([
        RoleEnum.ProjectSteward,
        RoleEnum.Admin,
        RoleEnum.EsaAdmin
    ])
    {
    }
}
