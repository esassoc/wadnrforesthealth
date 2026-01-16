using WADNR.API.Services;
using WADNR.EFModels.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SitkaCaptureService;
using System.Net;
using System.Threading.Tasks;

namespace WADNR.API.Controllers
{
    [ApiController]
    [Route("sitkacapture")]
    public class SitkaCaptureController : SitkaController<SitkaCaptureController>
    {
        private readonly SitkaCaptureService.SitkaCaptureService _sitkaCaptureService;

        public SitkaCaptureController(WADNRDbContext dbContext,
            ILogger<SitkaCaptureController> logger,
            KeystoneService keystoneService,
            IOptions<WADNRConfiguration> ltInfoConfiguration,
            SitkaCaptureService.SitkaCaptureService sitkaCaptureService) : base(dbContext, logger, keystoneService, ltInfoConfiguration)
        {
            _sitkaCaptureService = sitkaCaptureService;
        }

        [HttpPost("generate-pdf")]
        [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GeneratePdf([FromBody] CapturePostData capturePostData)
        {
            // turn this on to add debugging
            //capturePostData.timeoutInMilliseconds = 5000;
            //capturePostData.debug = true;
            //capturePostData.debugNetwork = true;
            var pdfBytes = await _sitkaCaptureService.PrintPDF(capturePostData);

            Response.Headers.Append("Content-Disposition", $"attachment; filename=template.pdf");
            return File(pdfBytes, "application/pdf");
        }
    }
}
