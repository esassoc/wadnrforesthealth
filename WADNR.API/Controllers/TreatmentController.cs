using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("treatments")]
public class TreatmentController(
    WADNRDbContext dbContext,
    ILogger<TreatmentController> logger,
    KeystoneService keystoneService,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<TreatmentController>(dbContext, logger, keystoneService, configuration)
{
    [HttpGet("{treatmentID}")]
    public async Task<ActionResult<TreatmentDetail>> GetByID([FromRoute] int treatmentID)
    {
        var treatment = await Treatments.GetByIDAsDetailAsync(DbContext, treatmentID);
        if (treatment == null)
        {
            return NotFound();
        }
        return Ok(treatment);
    }

    [HttpPost]
    public async Task<ActionResult<TreatmentDetail>> Create([FromBody] TreatmentUpsertRequest dto)
    {
        var treatment = await Treatments.CreateAsync(DbContext, dto);
        return Ok(treatment);
    }

    [HttpPut("{treatmentID}")]
    public async Task<ActionResult<TreatmentDetail>> Update([FromRoute] int treatmentID, [FromBody] TreatmentUpsertRequest dto)
    {
        var treatment = await Treatments.UpdateAsync(DbContext, treatmentID, dto);
        if (treatment == null)
        {
            return NotFound();
        }
        return Ok(treatment);
    }

    [HttpDelete("{treatmentID}")]
    public async Task<ActionResult> Delete([FromRoute] int treatmentID)
    {
        var deleted = await Treatments.DeleteAsync(DbContext, treatmentID);
        if (!deleted)
        {
            return NotFound();
        }
        return Ok();
    }
}
