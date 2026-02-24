using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
[Route("classifications")]
public class ClassificationController(
    WADNRDbContext dbContext,
    ILogger<ClassificationController> logger,
    IOptions<WADNRConfiguration> configuration,
    FileService fileService)
    : SitkaController<ClassificationController>(dbContext, logger, configuration)
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ClassificationGridRow>>> List()
    {
        var rows = await Classifications.ListAsGridRowAsync(DbContext);
        return Ok(rows);
    }

    [HttpGet("/with-project-count")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ClassificationWithProjectCount>>> ListWithProjectCount()
    {
        var rows = await Classifications.ListAsWithProjectCountAsync(DbContext);
        return Ok(rows);
    }

    [HttpGet("{classificationID}")]
    [AllowAnonymous]
    [EntityNotFound(typeof(Classification), "classificationID")]
    public async Task<ActionResult<ClassificationDetail>> Get([FromRoute] int classificationID)
    {
        var entity = await Classifications.GetByIDAsDetailAsync(DbContext, classificationID);
        if (entity == null)
        {
            return NotFound();
        }
        return Ok(entity);
    }

    [HttpPost]
    [AdminFeature]
    public async Task<ActionResult<ClassificationDetail>> Create([FromBody] ClassificationUpsertRequest dto)
    {
        var created = await Classifications.CreateAsync(DbContext, dto);
        if (created == null)
        {
            return BadRequest();
        }
        return CreatedAtAction(nameof(Get), new { classificationID = created.ClassificationID }, created);
    }

    [HttpPut("{classificationID}")]
    [AdminFeature]
    [EntityNotFound(typeof(Classification), "classificationID")]
    public async Task<ActionResult<ClassificationDetail>> Update([FromRoute] int classificationID, [FromBody] ClassificationUpsertRequest dto)
    {
        var updated = await Classifications.UpdateAsync(DbContext, classificationID, dto);
        if (updated == null)
        {
            return NotFound();
        }
        return Ok(updated);
    }

    [HttpDelete("{classificationID}")]
    [AdminFeature]
    [EntityNotFound(typeof(Classification), "classificationID")]
    public async Task<IActionResult> Delete([FromRoute] int classificationID)
    {
        var deleted = await Classifications.DeleteAsync(DbContext, classificationID);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpGet("{classificationID}/projects")]
    [ProjectViewFeature]
    public async Task<ActionResult<IEnumerable<ProjectClassificationDetailGridRow>>> ListProjectsForClassificationID([FromRoute] int classificationID)
    {
        var projects = await Projects.ListAsClassificationDetailGridRowForUserAsync(DbContext, classificationID, CallingUser);
        return Ok(projects);
    }

    [HttpPost("upload-key-image")]
    [AdminFeature]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<int>> UploadKeyImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is required.");
        }

        var fileResource = await fileService.CreateFileResource(DbContext, file, CallingUser.PersonID);
        return Ok(fileResource.FileResourceID);
    }
}
