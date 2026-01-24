using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows access to users who can manage programs.
/// Includes Admin, EsaAdmin, and CanEditProgram supplemental role.
/// </summary>
public class ProgramManageFeature : BaseAuthorizationAttribute
{
    public ProgramManageFeature() : base([
        RoleEnum.Admin,
        RoleEnum.EsaAdmin,
        RoleEnum.CanEditProgram
    ])
    {
    }
}
