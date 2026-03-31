using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using WADNR.API.Services;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
public class SystemInfoController(
    WADNRDbContext dbContext,
    ILogger<SystemInfoController> logger,
    IOptions<WADNRConfiguration> wadnrConfiguration)
    : SitkaController<SystemInfoController>(dbContext, logger, wadnrConfiguration)
{
    [Route("/")] // Default Route
    [HttpGet]
    [AllowAnonymous]
    public ActionResult<SystemInfoDetail> GetSystemInfo([FromServices] IWebHostEnvironment environment)
    {
        var systemInfo = new SystemInfoDetail
        {
            Environment = environment.EnvironmentName,
            CurrentTimeUTC = DateTime.UtcNow.ToString("o"),
            ScalarApiUrl = wadnrConfiguration.Value.ScalarApiUrl,
        };

        return Ok(systemInfo);
    }
}