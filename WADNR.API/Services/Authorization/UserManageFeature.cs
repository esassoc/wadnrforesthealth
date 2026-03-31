using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows access to users who can manage users, contacts, and organizations.
/// Includes Admin, EsaAdmin, and CanAddEditUsersContactsOrganizations supplemental role.
/// </summary>
public class UserManageFeature : BaseAuthorizationAttribute
{
    public UserManageFeature() : base([
        RoleEnum.Admin,
        RoleEnum.EsaAdmin,
        RoleEnum.CanAddEditUsersContactsOrganizations
    ])
    {
    }
}
