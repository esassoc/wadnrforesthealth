using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Restricts access to Admin and EsaAdmin roles only.
/// Use for system-wide administrative operations.
/// </summary>
public class AdminFeature : BaseAuthorizationAttribute
{
    public AdminFeature() : base([RoleEnum.Admin, RoleEnum.EsaAdmin])
    {
    }
}
