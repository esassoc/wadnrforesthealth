using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows a user to view a person record if:
/// - They are viewing their own profile (self-view), OR
/// - They have Normal+ role (same as NormalUserFeature)
///
/// This restores legacy UserViewFeature behavior where Unassigned users
/// could view their own profile page.
/// </summary>
public class PersonViewFeature : BaseAuthorizationAttribute
{
    public PersonViewFeature() : base([])
    {
    }

    protected override void OnAuthorizationCore(AuthorizationFilterContext context, WADNRDbContext dbContext, PersonDetail? person)
    {
        if (person == null)
        {
            context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
            return;
        }

        // Self-view is always allowed for any authenticated user
        if (context.RouteData.Values.TryGetValue("personID", out var routeValue)
            && int.TryParse(routeValue?.ToString(), out var targetPersonID)
            && targetPersonID == person.PersonID)
        {
            return;
        }

        // Otherwise require Normal+ (same roles as NormalUserFeature)
        var baseRoleID = person.BaseRole?.RoleID;
        if (baseRoleID is (int)RoleEnum.Normal
            or (int)RoleEnum.ProjectSteward
            or (int)RoleEnum.Admin
            or (int)RoleEnum.EsaAdmin)
        {
            return;
        }

        context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
    }
}
