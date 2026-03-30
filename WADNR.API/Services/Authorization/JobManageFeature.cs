using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows access to users who can manage finance API import jobs.
/// Includes ProjectSteward, Admin, and EsaAdmin roles.
/// Matches legacy JobManageFeature.
/// </summary>
public class JobManageFeature : BaseAuthorizationAttribute
{
    public JobManageFeature() : base([
        RoleEnum.ProjectSteward,
        RoleEnum.Admin,
        RoleEnum.EsaAdmin
    ])
    {
    }
}
