using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WADNR.API.Services;
using WADNR.Common.EMail;
using WADNR.EFModels.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WADNR.API.Hangfire;

public class ProgramNotificationJob(
    ILogger<ProgramNotificationJob> logger,
    IWebHostEnvironment webHostEnvironment,
    WADNRDbContext dbContext,
    IOptions<WADNRConfiguration> configuration,
    SitkaSmtpClientService sitkaSmtpClient)
    : ScheduledBackgroundJobBase<ProgramNotificationJob>(JobName, logger, webHostEnvironment, dbContext,
        configuration, sitkaSmtpClient)
{
    public const string JobName = "Program Notifications";

    public override List<RunEnvironment> RunEnvironments => new()
    {
        RunEnvironment.Production,
        RunEnvironment.Staging,
        //RunEnvironment.Development
    };

    protected override void RunJobImplementation()
    {
        RunNotificationsAsync().GetAwaiter().GetResult();
    }

    private async Task RunNotificationsAsync()
    {
        Logger.LogInformation("Processing {JobName}", JobName);

        var notifications = await ProgramNotifications.ProcessRemindersAsync(
            DbContext,
            WADNRConfiguration.SitkaSupportEmail,
            WADNRConfiguration.WebsiteDisplayName,
            WADNRConfiguration.WebUrl);

        // Send actual emails for each notification
        foreach (var notification in notifications)
        {
            if (!string.IsNullOrEmpty(notification.SentToPerson?.Email))
            {
                // Build a minimal email from the notification record
                // The actual email content was already built in ProcessRemindersAsync
                // We re-generate it here for sending
                var projects = notification.ProgramNotificationSentProjects.Select(x => x.Project).ToList();
                var mailMessage = ProgramNotifications.GenerateReminderEmail(
                    notification.SentToPerson,
                    projects,
                    notification.ProgramNotificationConfiguration.NotificationEmailText,
                    "Forest Health Tracker - Time to update your Project",
                    WADNRConfiguration.SitkaSupportEmail,
                    WADNRConfiguration.WebsiteDisplayName,
                    WADNRConfiguration.WebUrl);

                mailMessage.To.Add(new System.Net.Mail.MailAddress(notification.SentToPerson.Email));
                await sitkaSmtpClient.Send(mailMessage);
            }
        }

        Logger.LogInformation("Sent {Count} program notification emails", notifications.Count);
    }
}
