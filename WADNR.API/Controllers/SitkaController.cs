using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.Common.ExcelWorkbookUtilities;
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
    protected PersonDetail CallingUser => UserContext.GetUserAsDetailFromHttpContext(DbContext, HttpContext);

    protected ActionResult<TResult> RequireNotNullThrowNotFound<TResult>(TResult? value, string objectType, object objectID) where TResult : class
    {
        if (value is null)
        {
            var notFoundMessage = $"{objectType} with ID {objectID} does not exist!";
            Logger.LogError(notFoundMessage);
            return NotFound(notFoundMessage);
        }
        return Ok(value);
    }

    protected IActionResult DeleteOrNotFound(bool deleted)
        => deleted ? NoContent() : NotFound();

    protected IActionResult ExcelFileResult(ExcelWorkbookMaker wbm, string fileName)
    {
        var workbook = wbm.ToXLWorkbook();
        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Seek(0, SeekOrigin.Begin);
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}
