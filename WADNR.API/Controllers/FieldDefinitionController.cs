using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.API.Services.Attributes;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("field-definitions")]
public class FieldDefinitionController(
    WADNRDbContext dbContext,
    ILogger<FieldDefinitionController> logger,
    KeystoneService keystoneService,
    IOptions<WADNRConfiguration> configuration)
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