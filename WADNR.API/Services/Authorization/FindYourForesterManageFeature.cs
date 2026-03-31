using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows access to users who can manage forester work unit assignments.
/// Includes ProjectSteward, Admin, and EsaAdmin roles.
/// Matches legacy FindYourForesterManageFeature.
/// </summary>
public class FindYourForesterManageFeature : BaseAuthorizationAttribute
{
    public FindYourForesterManageFeature() : base([
        RoleEnum.ProjectSteward,
        RoleEnum.Admin,
        RoleEnum.EsaAdmin
    ])
    {
    }
}
