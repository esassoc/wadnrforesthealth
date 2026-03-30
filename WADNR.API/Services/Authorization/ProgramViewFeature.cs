using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Authorization attribute for program viewing endpoints.
/// Allows anonymous access but still populates user context for authenticated users.
/// Same pattern as ProjectViewFeature.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class ProgramViewFeature : Attribute, IAuthorizationFilter, IAllowAnonymous
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // This filter intentionally does NOT block any requests.
        // It allows anonymous access while ensuring that if a user IS authenticated,
        // their claims are available via HttpContext.User.
    }
}
