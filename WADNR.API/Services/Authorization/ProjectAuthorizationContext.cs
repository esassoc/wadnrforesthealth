using System.Linq;
using Microsoft.AspNetCore.Http;
using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Scoped DI service that resolves projectID from route values and loads authorization-relevant data.
/// Constructed once per request, reusable by both auth attributes and controllers.
/// Pattern: LTInfo's TdrListingContext.
/// </summary>
public class ProjectAuthorizationContext
{
    public int? ProjectID { get; }
    public ProjectAuthorizationData? AuthData { get; }
    public int? StewardshipAreaTypeID { get; }

    public ProjectAuthorizationContext(WADNRDbContext dbContext, HttpContext httpContext)
    {
        if (httpContext.Request.RouteValues.TryGetValue("projectID", out var idObj)
            && int.TryParse(idObj?.ToString(), out var projectID))
        {
            ProjectID = projectID;
            AuthData = ProjectAuthorizationData.Load(dbContext, projectID);
            StewardshipAreaTypeID = dbContext.SystemAttributes
                .Select(sa => sa.ProjectStewardshipAreaTypeID)
                .FirstOrDefault();
        }
    }

    public bool HasProject => AuthData != null;
}
