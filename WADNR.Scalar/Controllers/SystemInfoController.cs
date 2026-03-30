using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using System;
using WADNR.Scalar.Logging;

namespace WADNR.Scalar.Controllers;

[ApiController]
[ExcludeFromApiReference]
public class SystemInfoController : ControllerBase
{
    [HttpGet("/", Name = "GetSystemInfo")]
    [AllowAnonymous]
    [LogIgnore]
    public IActionResult GetSystemInfo([FromServices] IWebHostEnvironment environment)
    {
        return Ok(new
        {
            Environment = environment.EnvironmentName,
            CurrentTimeUTC = DateTime.UtcNow.ToString("o")
        });
    }
}
