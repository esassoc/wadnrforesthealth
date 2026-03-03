using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows access to users who can manage treatments.
/// Includes Admin, EsaAdmin, and CanManageFundSourcesAndAgreements supplemental role.
/// Matches legacy TreatmentEditAsAdminFeature.
/// </summary>
public class TreatmentManageFeature : BaseAuthorizationAttribute
{
    public TreatmentManageFeature() : base([
        RoleEnum.Admin,
        RoleEnum.EsaAdmin,
        RoleEnum.CanManageFundSourcesAndAgreements
    ])
    {
    }
}
