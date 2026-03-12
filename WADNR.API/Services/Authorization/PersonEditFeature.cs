using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows a user to edit a person record if:
/// - They are editing their own profile (self-edit), OR
/// - They have contact/user management permission (same rules as ContactManageFeature)
///
/// Used on PUT /people/{personID} to restore legacy self-edit behavior.
/// </summary>
public class PersonEditFeature : BaseAuthorizationAttribute
{
    private const int WadnrOrganizationID = 4704;

    public PersonEditFeature() : base([
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

        // Check for self-edit: if the personID in the route matches the calling user
        if (context.RouteData.Values.TryGetValue("personID", out var routeValue)
            && int.TryParse(routeValue?.ToString(), out var targetPersonID)
            && targetPersonID == person.PersonID)
        {
            return; // Self-edit is always allowed
        }

        // Otherwise, fall back to ContactManageFeature logic:
        // Admin/EsaAdmin bypass
        if (person.BaseRole?.RoleID is (int)RoleEnum.Admin or (int)RoleEnum.EsaAdmin)
        {
            return;
        }

        // CanAddEditUsersContactsOrganizations supplemental role
        if (person.SupplementalRoleList.Any(r => r.RoleID == (int)RoleEnum.CanAddEditUsersContactsOrganizations))
        {
            return;
        }

        // ProjectSteward
        if (person.BaseRole?.RoleID == (int)RoleEnum.ProjectSteward)
        {
            return;
        }

        // Normal user from WADNR org
        if (person.BaseRole?.RoleID == (int)RoleEnum.Normal && person.OrganizationID == WadnrOrganizationID)
        {
            return;
        }

        context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
    }
}
