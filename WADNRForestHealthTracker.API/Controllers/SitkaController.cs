using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNRForestHealthTracker.API.Services;
using WADNRForestHealthTracker.EFModels.Entities;
using WADNRForestHealthTracker.Models.DataTransferObjects;

namespace WADNRForestHealthTracker.API.Controllers;

public abstract class SitkaController<T>(
    WADNRForestHealthTrackerDbContext dbContext,
    ILogger<T> logger,
    KeystoneService keystoneService,
    IOptions<WADNRForestHealthTrackerConfiguration> configuration)
    : ControllerBase
{
    protected readonly WADNRForestHealthTrackerDbContext DbContext = dbContext;
    protected readonly ILogger<T> Logger = logger;
    protected readonly KeystoneService KeystoneService = keystoneService;
    protected readonly WADNRForestHealthTrackerConfiguration Configuration = configuration.Value;
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