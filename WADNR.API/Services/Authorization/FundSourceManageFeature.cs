using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows access to users who can manage fund sources and allocations.
/// Includes Admin, EsaAdmin, and CanManageFundSourcesAndAgreements supplemental role.
/// </summary>
public class FundSourceManageFeature : BaseAuthorizationAttribute
{
    public FundSourceManageFeature() : base([
        RoleEnum.Admin,
        RoleEnum.EsaAdmin,
        RoleEnum.CanManageFundSourcesAndAgreements
    ])
    {
    }
}
