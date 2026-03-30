using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows access to users who can view filled-in cost share agreements.
/// Requires Admin, EsaAdmin, or CanViewLandownerInfo supplemental role.
/// </summary>
public class CostShareViewFeature : BaseAuthorizationAttribute
{
    public CostShareViewFeature() : base([RoleEnum.Admin, RoleEnum.EsaAdmin, RoleEnum.CanViewLandownerInfo])
    {
    }
}
