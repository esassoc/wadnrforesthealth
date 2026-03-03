using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows access to users who can manage programs (create, update, delete).
/// Includes Admin and EsaAdmin roles only.
/// </summary>
public class ProgramManageFeature : BaseAuthorizationAttribute
{
    public ProgramManageFeature() : base([
        RoleEnum.Admin,
        RoleEnum.EsaAdmin
    ])
    {
    }
}
