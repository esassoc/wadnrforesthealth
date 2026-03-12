using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects.ProjectUpdate;

namespace WADNR.API.Controllers;

[ApiController]
[Route("project-update-configurations")]
public class ProjectUpdateConfigurationController(
    WADNRDbContext dbContext,
    ILogger<ProjectUpdateConfigurationController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<ProjectUpdateConfigurationController>(dbContext, logger, configuration)
{
    [HttpGet]
    [AdminFeature]
    public async Task<ActionResult<ProjectUpdateConfigurationDetail>> GetConfiguration()
    {
        var config = await ProjectUpdateConfigurations.GetConfigurationAsync(DbContext);
        if (config == null)
        {
            return NotFound();
        }
        return Ok(config);
    }

    [HttpPut]
    [AdminFeature]
    public async Task<ActionResult> UpdateConfiguration([FromBody] ProjectUpdateConfigurationUpsertRequest request)
    {
        await ProjectUpdateConfigurations.UpdateConfigurationAsync(DbContext, request);
        return NoContent();
    }

    [HttpGet("email-preview/{emailType}")]
    [AdminFeature]
    public async Task<ActionResult<EmailContentPreview>> GetEmailContentPreview([FromRoute] string emailType)
    {
        var validTypes = new[] { "kickoff", "reminder", "closeout" };
        if (!validTypes.Contains(emailType.ToLowerInvariant()))
        {
            return BadRequest("Invalid email type. Must be 'kickoff', 'reminder', or 'closeout'.");
        }

        var preview = await ProjectUpdateConfigurations.GetEmailContentPreviewAsync(DbContext, emailType, Configuration.WebUrl);
        if (preview == null)
        {
            return NotFound("Project update configuration not found.");
        }

        return Ok(preview);
    }
}
