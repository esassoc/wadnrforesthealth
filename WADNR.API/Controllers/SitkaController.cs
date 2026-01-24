using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

public abstract class SitkaController<T>(
    WADNRDbContext dbContext,
    ILogger<T> logger,
    IOptions<WADNRConfiguration> configuration)
    : ControllerBase
{
    protected readonly WADNRDbContext DbContext = dbContext;
    protected readonly ILogger<T> Logger = logger;
    protected readonly WADNRConfiguration Configuration = configuration.Value;
    protected PersonDetail CallingUser => UserContext.GetUserFromHttpContext(DbContext, HttpContext);

    protected ActionResult RequireNotNullThrowNotFound(object theObject, string objectType, object objectID)
    {
        return ThrowNotFound(theObject, objectType, objectID, out var actionResult) ? actionResult : Ok(theObject);
    }

    protected bool ThrowNotFound(object theObject, string objectType, object objectID, out ActionResult actionResult)
    {
        if (theObject == null)
        {
            var notFoundMessage = $"{objectType} with ID {objectID} does not exist!";
            Logger.LogError(notFoundMessage);
            {
                actionResult = NotFound(notFoundMessage);
                return true;
            }
        }

        actionResult = null;
        return false;
    }
}