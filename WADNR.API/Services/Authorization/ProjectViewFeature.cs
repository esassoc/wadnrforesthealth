using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Authorization attribute for project viewing endpoints.
/// Allows anonymous access but still populates user context for authenticated users.
/// Mirrors legacy ProjectViewFeature behavior.
/// </summary>
/// <remarks>
/// This attribute is an IAuthorizationFilter but does NOT inherit from AuthorizeAttribute.
/// This means:
/// - Authentication middleware still runs and populates HttpContext.User for authenticated requests
/// - No authorization check is performed (all requests pass through)
/// - Anonymous requests are allowed through
///
/// The actual visibility filtering is handled by ProjectVisibility.ApplyVisibilityFilter()
/// in the static helpers, which uses CallingUser to determine what the user can see.
///
/// Unlike [AllowAnonymous], this attribute doesn't skip authentication middleware,
/// so authenticated users will have their claims populated.
/// </remarks>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class ProjectViewFeature : Attribute, IAuthorizationFilter, IAllowAnonymous
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // This filter intentionally does NOT block any requests.
        // It allows anonymous access while ensuring that if a user IS authenticated,
        // their claims are available via HttpContext.User.
    }
}
