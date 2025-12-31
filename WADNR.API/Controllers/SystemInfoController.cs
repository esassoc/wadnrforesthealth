using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
public class SystemInfoController(
    WADNRDbContext dbContext,
    ILogger<SystemInfoController> logger,
    KeystoneService keystoneService,
    IOptions<WADNRConfiguration> wadnrConfiguration)
    : SitkaController<SystemInfoController>(dbContext, logger, keystoneService, wadnrConfiguration)
{
    [Route("/")] // Default Route
    [HttpGet]
    public ActionResult<SystemInfoDetail> GetSystemInfo([FromServices] IWebHostEnvironment environment)
    {
        var systemInfo = new SystemInfoDetail
        {
            Environment = environment.EnvironmentName,
            CurrentTimeUTC = DateTime.UtcNow.ToString("o"),
        };

        return Ok(systemInfo);
    }
}