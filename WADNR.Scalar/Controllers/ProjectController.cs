using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects.Project;

namespace WADNR.Scalar.Controllers;

[ApiController]
[Route("projects")]
[Authorize]
public class ProjectController(WADNRDbContext dbContext) : ControllerBase
{
    /// <summary>
    /// Returns all forest health projects, including project type, stage, primary contact, focus area, approval status, and location coordinates.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ProjectApiJson>>> List()
    {
        var items = await Projects.ListAsApiJsonAsync(dbContext);
        return Ok(items);
    }
}
