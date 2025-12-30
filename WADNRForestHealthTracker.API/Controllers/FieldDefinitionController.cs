using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNRForestHealthTracker.API.Services;
using WADNRForestHealthTracker.API.Services.Attributes;
using WADNRForestHealthTracker.EFModels.Entities;
using WADNRForestHealthTracker.Models.DataTransferObjects;

namespace WADNRForestHealthTracker.API.Controllers;

[ApiController]
[Route("field-definitions")]
public class FieldDefinitionController(
    WADNRForestHealthTrackerDbContext dbContext,
    ILogger<FieldDefinitionController> logger,
    KeystoneService keystoneService,
    IOptions<WADNRForestHealthTrackerConfiguration> configuration)
    : SitkaController<FieldDefinitionController>(dbContext, logger, keystoneService, configuration)
{
    [HttpGet]
    public async Task<ActionResult<List<FieldDefinitionDatum>>> List()
    {
        var entities = await FieldDefinitionData.ListAsDetailAsync(DbContext);
        return Ok(entities);
    }


    [HttpGet("{fieldDefinitionID}")]
    [EntityNotFound(typeof(FieldDefinitionDatum), "fieldDefinitionID")]
    public async Task<ActionResult<FieldDefinitionDatumDetail>> Get([FromRoute] int fieldDefinitionID)
    {
        var entity = await FieldDefinitionData.GetByFieldDefinitionAsDetailAsync(DbContext, fieldDefinitionID);
        return Ok(entity);
    }
}