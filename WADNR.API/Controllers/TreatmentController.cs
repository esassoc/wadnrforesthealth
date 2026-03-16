using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.API.Services.Authorization;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("treatments")]
public class TreatmentController(
    WADNRDbContext dbContext,
    ILogger<TreatmentController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<TreatmentController>(dbContext, logger, configuration)
{
    [HttpGet("{treatmentID}")]
    [AllowAnonymous]
    public async Task<ActionResult<TreatmentDetail>> GetByID([FromRoute] int treatmentID)
    {
        var treatment = await Treatments.GetByIDAsDetailAsync(DbContext, treatmentID);
        return RequireNotNullThrowNotFound(treatment, "Treatment", treatmentID);
    }

    [HttpPost]
    [TreatmentManageFeature]
    public async Task<ActionResult<TreatmentDetail>> Create([FromBody] TreatmentUpsertRequest dto)
    {
        var treatment = await Treatments.CreateAsync(DbContext, dto);
        return Ok(treatment);
    }

    [HttpPut("{treatmentID}")]
    [TreatmentManageFeature]
    public async Task<ActionResult<TreatmentDetail>> Update([FromRoute] int treatmentID, [FromBody] TreatmentUpsertRequest dto)
    {
        var treatment = await Treatments.UpdateAsync(DbContext, treatmentID, dto);
        return RequireNotNullThrowNotFound(treatment, "Treatment", treatmentID);
    }

    [HttpDelete("{treatmentID}")]
    [TreatmentManageFeature]
    public async Task<IActionResult> Delete([FromRoute] int treatmentID)
    {
        var deleted = await Treatments.DeleteAsync(DbContext, treatmentID);
        return DeleteOrNotFound(deleted);
    }
}
