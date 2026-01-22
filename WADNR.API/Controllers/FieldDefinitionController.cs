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
    //MP 1/2/26 This doesn't work here because FieldDefinitionID is not the PK for FieldDefinitionDatum
    //[EntityNotFound(typeof(FieldDefinitionDatum), "fieldDefinitionID")]
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
            FieldDefinitionDatumValue = string.Empty,
            FieldDefinition = fieldDefinition != null
                ? new FieldDefinitionDetail
                {
                    FieldDefinitionID = fieldDefinition.FieldDefinitionID,
                    FieldDefinitionName = fieldDefinition.FieldDefinitionName,
                    FieldDefinitionDisplayName = fieldDefinition.FieldDefinitionDisplayName
                }
                : new FieldDefinitionDetail
                {
                    FieldDefinitionID = fieldDefinitionID,
                    FieldDefinitionName = string.Empty,
                    FieldDefinitionDisplayName = string.Empty
                }
        });
    }
}