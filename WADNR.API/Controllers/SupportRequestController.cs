using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WADNR.API.Services;
using WADNR.Common.EMail;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Controllers;

[ApiController]
[Route("support-requests")]
public class SupportRequestController(
    WADNRDbContext dbContext,
    ILogger<SupportRequestController> logger,
    IOptions<WADNRConfiguration> configuration,
    SitkaSmtpClientService sitkaSmtpClient)
    : SitkaController<SupportRequestController>(dbContext, logger, configuration)
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SupportRequestCreateDto dto)
    {
        // Get the current person entity
        var currentPerson = await DbContext.People.FindAsync(CallingUser.PersonID);
        if (currentPerson == null)
        {
            return Unauthorized("Unable to identify current user.");
        }

        // Create the support request log entry
        var supportRequestLog = await SupportRequestLogs.CreateAsync(DbContext, dto, currentPerson);

        // Build and send the email
        var mailMessage = SupportRequestLogs.BuildFeedbackEmail(
            supportRequestLog,
            dto.CurrentPageUrl,
            Configuration.SitkaSupportEmail);

        // Get support email recipients
        var recipients = await SupportRequestLogs.GetSupportEmailRecipientsAsync(DbContext);

        if (recipients.Count == 0)
        {
            // If no support email recipients configured, use the default support email
            recipients.Add(Configuration.SitkaSupportEmail);
            mailMessage.Body = $"<p style=\"font-weight:bold\">Note: No users are currently configured to receive support emails. Defaulting to: {Configuration.SitkaSupportEmail}</p>{mailMessage.Body}";
        }

        foreach (var recipient in recipients)
        {
            mailMessage.To.Add(new MailAddress(recipient));
        }

        try
        {
            await sitkaSmtpClient.Send(mailMessage);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to send support request email");
            // Don't fail the request - the support log was still created
        }

        return Ok(new { Message = "Feedback submitted successfully." });
    }
}
