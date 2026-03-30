using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows access to users who can manage page content (custom pages, field definitions, etc.).
/// Includes Admin, EsaAdmin, and CanManagePageContent supplemental role.
/// </summary>
public class PageContentManageFeature : BaseAuthorizationAttribute
{
    public PageContentManageFeature() : base([
        RoleEnum.Admin,
        RoleEnum.EsaAdmin,
        RoleEnum.CanManagePageContent
    ])
    {
    }
}
