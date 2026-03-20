using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.API.Services.Attributes;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("field-definitions")]
public class FieldDefinitionController(
    WADNRDbContext dbContext,
    ILogger<FieldDefinitionController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<FieldDefinitionController>(dbContext, logger, configuration)
{
    [HttpGet]
    [AdminFeature]
    public async Task<ActionResult<List<FieldDefinitionDatumDetail>>> List()
    {
        var entities = await FieldDefinitionData.ListAsDetailAsync(DbContext);
        return Ok(entities);
    }


    [HttpGet("{fieldDefinitionID}")]
    [AllowAnonymous]
    public async Task<ActionResult<FieldDefinitionDatumDetail>> Get([FromRoute] int fieldDefinitionID)
    {
        var entity = await FieldDefinitionData.GetByFieldDefinitionAsDetailAsync(DbContext, fieldDefinitionID);
        if (entity != null)
        {
            return Ok(entity);
        }

        // Return an empty definition if the FieldDefinitionDatum doesn't exist
        // This prevents 404s when field definitions haven't been populated yet
        var fieldDefinition = FieldDefinition.AllLookupDictionary.GetValueOrDefault(fieldDefinitionID);
        return Ok(new FieldDefinitionDatumDetail
        {
            FieldDefinitionID = fieldDefinitionID,
            FieldDefinitionDatumValue = fieldDefinition?.DefaultDefinition ?? string.Empty,
            FieldDefinition = fieldDefinition != null
                ? new FieldDefinitionDetail
                {
                    FieldDefinitionID = fieldDefinition.FieldDefinitionID,
                    FieldDefinitionName = fieldDefinition.FieldDefinitionName,
                    FieldDefinitionDisplayName = fieldDefinition.FieldDefinitionDisplayName,
                    DefaultDefinition = fieldDefinition.DefaultDefinition
                }
                : new FieldDefinitionDetail
                {
                    FieldDefinitionID = fieldDefinitionID,
                    FieldDefinitionName = string.Empty,
                    FieldDefinitionDisplayName = string.Empty
                }
        });
    }

    [HttpPut("{fieldDefinitionID}")]
    [PageContentManageFeature]
    public async Task<ActionResult<FieldDefinitionDatumDetail>> Update(
        [FromRoute] int fieldDefinitionID,
        [FromBody] FieldDefinitionDatumUpsertRequest upsertRequest)
    {
        var updated = await FieldDefinitionData.Update(DbContext, fieldDefinitionID, upsertRequest);
        if (updated == null)
        {
            return NotFound();
        }
        return Ok(updated);
    }
}