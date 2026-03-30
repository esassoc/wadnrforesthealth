using Microsoft.AspNetCore.Http;
using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Scoped DI service that resolves programID from route values.
/// Pattern: LTInfo's TdrListingContext.
/// </summary>
public class ProgramAuthorizationContext
{
    public int? ProgramID { get; }

    public ProgramAuthorizationContext(WADNRDbContext dbContext, HttpContext httpContext)
    {
        if (httpContext.Request.RouteValues.TryGetValue("programID", out var idObj)
            && int.TryParse(idObj?.ToString(), out var programID))
        {
            ProgramID = programID;
        }
    }

    public bool HasProgram => ProgramID.HasValue;
}
