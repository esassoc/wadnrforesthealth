using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows access to users who can manage agreements.
/// Includes Admin, EsaAdmin, and CanManageFundSourcesAndAgreements supplemental role.
/// </summary>
public class AgreementManageFeature : BaseAuthorizationAttribute
{
    public AgreementManageFeature() : base([
        RoleEnum.Admin,
        RoleEnum.EsaAdmin,
        RoleEnum.CanManageFundSourcesAndAgreements
    ])
    {
    }
}
