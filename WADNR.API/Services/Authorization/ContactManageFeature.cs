using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows access to users who can manage contacts (non-full-user people records).
/// Mirrors legacy ContactManageFeature behavior:
/// - Admin/EsaAdmin: always allowed
/// - CanAddEditUsersContactsOrganizations supplemental role: always allowed
/// - ProjectSteward: always allowed
/// - Normal user: allowed only if they belong to WADNR organization (ID 4704)
/// </summary>
public class ContactManageFeature : BaseAuthorizationAttribute
{
    private const int WadnrOrganizationID = 4704;

    public ContactManageFeature() : base([
        RoleEnum.Normal,
        RoleEnum.ProjectSteward,
        RoleEnum.Admin,
        RoleEnum.EsaAdmin,
        RoleEnum.CanAddEditUsersContactsOrganizations
    ])
    {
    }

    protected override void OnAuthorizationCore(AuthorizationFilterContext context, WADNRDbContext dbContext, PersonDetail? person)
    {
        if (person == null)
        {
            context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
            return;
        }

        // Admin/EsaAdmin bypass all checks
        if (person.BaseRole?.RoleID is (int)RoleEnum.Admin or (int)RoleEnum.EsaAdmin)
        {
            return;
        }

        // CanAddEditUsersContactsOrganizations supplemental role: always allowed
        if (person.SupplementalRoleList.Any(r => r.RoleID == (int)RoleEnum.CanAddEditUsersContactsOrganizations))
        {
            return;
        }

        // ProjectSteward: always allowed
        if (person.BaseRole?.RoleID == (int)RoleEnum.ProjectSteward)
        {
            return;
        }

        // Normal user: allowed only if from WADNR org
        if (person.BaseRole?.RoleID == (int)RoleEnum.Normal && person.OrganizationID == WadnrOrganizationID)
        {
            return;
        }

        // Otherwise: forbidden
        context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
    }
}
