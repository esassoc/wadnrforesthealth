using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows access to users who can view vendor information.
/// Requires Admin, EsaAdmin, or ProjectSteward role.
/// </summary>
public class VendorViewFeature : BaseAuthorizationAttribute
{
    public VendorViewFeature() : base([RoleEnum.Admin, RoleEnum.EsaAdmin, RoleEnum.ProjectSteward])
    {
    }
}
