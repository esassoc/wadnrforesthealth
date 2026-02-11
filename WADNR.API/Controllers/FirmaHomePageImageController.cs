using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("firma-home-page-images")]
public class FirmaHomePageImageController(
    WADNRDbContext dbContext,
    ILogger<FirmaHomePageImageController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<FirmaHomePageImageController>(dbContext, logger, configuration)
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<FirmaHomePageImageDetail>>> List()
    {
        var images = await FirmaHomePageImages.ListAsync(DbContext);
        return Ok(images);
    }
}
