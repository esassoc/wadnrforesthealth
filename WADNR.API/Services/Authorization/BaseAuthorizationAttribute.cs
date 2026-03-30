using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Base authorization attribute that checks if the current user has one of the granted roles.
/// Derive from this class to create feature-specific authorization attributes.
/// </summary>
public abstract class BaseAuthorizationAttribute(IEnumerable<RoleEnum> grantedRoles)
    : AuthorizeAttribute, IAuthorizationFilter
{
    public int Order { get; set; } = 0;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            // Let the default [Authorize] behavior handle unauthenticated users (401)
            return;
        }

        var dbContextService = context.HttpContext.RequestServices.GetService(typeof(WADNRDbContext));
        if (dbContextService is not WADNRDbContext dbContext)
        {
            throw new ApplicationException(
                "Could not find injected WADNRDbContext. BaseAuthorizationAttribute needs the DbContext registered.");
        }

        var person = UserContext.GetUserAsDetailFromHttpContext(dbContext, context.HttpContext);

        // Check if user has any of the granted roles (base role or supplemental roles)
        var isAuthorized = person != null && HasAnyGrantedRole(person, grantedRoles);

        if (!isAuthorized)
        {
            context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
            return;
        }

        // Call extension point for entity/context-specific logic
        OnAuthorizationCore(context, dbContext, person);
    }

    /// <summary>
    /// Checks if the person has any of the granted roles.
    /// Considers both the base role and any supplemental roles the person may have.
    /// </summary>
    private static bool HasAnyGrantedRole(PersonDetail person, IEnumerable<RoleEnum> grantedRoles)
    {
        var grantedRoleIds = grantedRoles.Select(r => (int)r).ToHashSet();

        // If no roles are specified, allow all authenticated users
        if (grantedRoleIds.Count == 0)
        {
            return true;
        }

        // Check base role
        if (person.BaseRole != null && grantedRoleIds.Contains(person.BaseRole.RoleID))
        {
            return true;
        }

        // Check supplemental roles
        foreach (var supplementalRole in person.SupplementalRoleList)
        {
            if (grantedRoleIds.Contains(supplementalRole.RoleID))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Extension point for derived classes to add entity/context-specific authorization logic.
    /// </summary>
    protected virtual void OnAuthorizationCore(AuthorizationFilterContext context, WADNRDbContext dbContext, PersonDetail? person)
    {
        // Default: no additional checks
    }
}
