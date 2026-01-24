using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.ProgramIndex;

namespace WADNR.API.Controllers;

[ApiController]
[Route("program-indices")]
public class ProgramIndexController(
    WADNRDbContext dbContext,
    ILogger<ProgramIndexController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<ProgramIndexController>(dbContext, logger, configuration)
{
    [HttpGet]
    public async Task<ActionResult<List<ProgramIndexGridRow>>> List()
    {
        var programIndices = await ProgramIndices.ListAsGridRowAsync(DbContext);
        return Ok(programIndices);
    }

    [HttpGet("{programIndexID}")]
    public async Task<ActionResult<ProgramIndexDetail>> GetByID([FromRoute] int programIndexID)
    {
        var programIndex = await ProgramIndices.GetByIDAsDetailAsync(DbContext, programIndexID);
        if (programIndex == null)
        {
            return NotFound();
        }
        return Ok(programIndex);
    }

    [HttpGet("for-biennium/{biennium}")]
    public async Task<ActionResult<List<ProgramIndexGridRow>>> ListForBiennium([FromRoute] int biennium)
    {
        var programIndices = await ProgramIndices.ListForBienniumAsGridRowAsync(DbContext, biennium);
        return Ok(programIndices);
    }

    [HttpGet("lookup")]
    public async Task<ActionResult<List<ProgramIndexLookupItem>>> ListLookup()
    {
        var programIndices = await ProgramIndices.ListAsLookupItemAsync(DbContext);
        return Ok(programIndices);
    }

    [HttpGet("lookup/for-biennium/{biennium}")]
    public async Task<ActionResult<List<ProgramIndexLookupItem>>> ListLookupForBiennium([FromRoute] int biennium)
    {
        var programIndices = await ProgramIndices.ListForBienniumAsLookupItemAsync(DbContext, biennium);
        return Ok(programIndices);
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<ProgramIndexLookupItem>>> Search([FromQuery] string? term, [FromQuery] int? biennium)
    {
        var programIndices = await ProgramIndices.SearchAsLookupItemAsync(DbContext, term ?? string.Empty, biennium);
        return Ok(programIndices);
    }
}
