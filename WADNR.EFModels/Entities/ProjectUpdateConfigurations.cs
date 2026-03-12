using System.Text;
using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects.ProjectUpdate;

namespace WADNR.EFModels.Entities;

public static class ProjectUpdateConfigurations
{
    private const string EmailTemplate = @"Hello {0},<br/><br/>
{1}
<div style=""font-weight:bold"">Your <a href=""{2}"">projects that require an update</a> are:</div>
<div style=""margin-left: 15px"">
    {3}
</div>";

    public static async Task<ProjectUpdateConfigurationDetail?> GetConfigurationAsync(WADNRDbContext dbContext)
    {
        return await dbContext.ProjectUpdateConfigurations
            .AsNoTracking()
            .Select(ProjectUpdateConfigurationProjections.AsDetail)
            .FirstOrDefaultAsync();
    }

    public static async Task UpdateConfigurationAsync(
        WADNRDbContext dbContext,
        ProjectUpdateConfigurationUpsertRequest request)
    {
        var config = await dbContext.ProjectUpdateConfigurations.FirstOrDefaultAsync();
        if (config == null)
        {
            config = new ProjectUpdateConfiguration();
            dbContext.ProjectUpdateConfigurations.Add(config);
        }

        config.EnableProjectUpdateReminders = request.EnableProjectUpdateReminders;
        config.ProjectUpdateKickOffDate = request.ProjectUpdateKickOffDate;
        config.ProjectUpdateKickOffIntroContent = request.ProjectUpdateKickOffIntroContent;
        config.SendPeriodicReminders = request.SendPeriodicReminders;
        config.ProjectUpdateReminderInterval = request.ProjectUpdateReminderInterval;
        config.ProjectUpdateReminderIntroContent = request.ProjectUpdateReminderIntroContent;
        config.SendCloseOutNotification = request.SendCloseOutNotification;
        config.ProjectUpdateCloseOutDate = request.ProjectUpdateCloseOutDate;
        config.ProjectUpdateCloseOutIntroContent = request.ProjectUpdateCloseOutIntroContent;

        await dbContext.SaveChangesAsync();
    }

    public static async Task<List<PeopleReceivingReminderGridRow>> ListPeopleReceivingRemindersAsync(
        WADNRDbContext dbContext)
    {
        var primaryContactRelationshipTypeID = ProjectPersonRelationshipType.PrimaryContact.ProjectPersonRelationshipTypeID;

        var reportingPeriodStart = new DateTime(DateTime.Today.Year, 1, 1);

        // Get all approved projects and their primary contacts + batch state info
        var projectData = await dbContext.Projects
            .AsNoTracking()
            .Where(p => p.ProjectApprovalStatusID == (int)ProjectApprovalStatusEnum.Approved)
            .Select(p => new
            {
                p.ProjectID,
                PrimaryContactPersonID = p.ProjectPeople
                    .Where(pp => pp.ProjectPersonRelationshipTypeID == primaryContactRelationshipTypeID)
                    .Select(pp => (int?)pp.PersonID)
                    .FirstOrDefault(),
                LatestNonApprovedBatchStateID = p.ProjectUpdateBatches
                    .Where(b => b.ProjectUpdateStateID != (int)ProjectUpdateStateEnum.Approved)
                    .OrderByDescending(b => b.LastUpdateDate)
                    .Select(b => (int?)b.ProjectUpdateStateID)
                    .FirstOrDefault(),
                LatestApprovedBatchDate = p.ProjectUpdateBatches
                    .Where(b => b.ProjectUpdateStateID == (int)ProjectUpdateStateEnum.Approved)
                    .Max(b => (DateTime?)b.LastUpdateDate),
            })
            .Where(p => p.PrimaryContactPersonID != null)
            .ToListAsync();

        // Get person info for all primary contacts
        var personIds = projectData.Select(p => p.PrimaryContactPersonID!.Value).Distinct().ToList();
        var people = await dbContext.People
            .AsNoTracking()
            .Where(p => personIds.Contains(p.PersonID))
            .Select(p => new
            {
                p.PersonID,
                PersonName = p.FirstName + " " + p.LastName,
                OrganizationID = (int?)p.OrganizationID,
                OrganizationName = p.Organization.OrganizationName,
                OrganizationShortName = p.Organization.OrganizationShortName,
                p.Email,
            })
            .ToListAsync();

        // Get reminder notification counts per person (scoped to current reporting period)
        var reminderCounts = await dbContext.Notifications
            .AsNoTracking()
            .Where(n => n.NotificationTypeID == (int)NotificationTypeEnum.ProjectUpdateReminder
                && personIds.Contains(n.PersonID)
                && n.NotificationDate >= reportingPeriodStart)
            .GroupBy(n => n.PersonID)
            .Select(g => new
            {
                PersonID = g.Key,
                Count = g.Count(),
                LastDate = g.Max(n => n.NotificationDate),
            })
            .ToListAsync();

        var reminderLookup = reminderCounts.ToDictionary(r => r.PersonID);

        // Group projects by primary contact and compute counts
        var grouped = projectData
            .GroupBy(p => p.PrimaryContactPersonID!.Value)
            .Select(g =>
            {
                var personInfo = people.First(p => p.PersonID == g.Key);
                reminderLookup.TryGetValue(g.Key, out var reminders);

                return new PeopleReceivingReminderGridRow
                {
                    PersonID = g.Key,
                    PersonName = personInfo.PersonName,
                    OrganizationID = personInfo.OrganizationID,
                    OrganizationName = personInfo.OrganizationName,
                    OrganizationShortName = personInfo.OrganizationShortName,
                    Email = personInfo.Email,
                    ProjectsRequiringUpdate = g.Count(),
                    UpdatesNotStarted = g.Count(p => p.LatestNonApprovedBatchStateID == null
                        && (p.LatestApprovedBatchDate == null || p.LatestApprovedBatchDate < reportingPeriodStart)),
                    UpdatesInProgress = g.Count(p => p.LatestNonApprovedBatchStateID == (int)ProjectUpdateStateEnum.Created),
                    UpdatesSubmitted = g.Count(p => p.LatestNonApprovedBatchStateID == (int)ProjectUpdateStateEnum.Submitted),
                    UpdatesReturned = g.Count(p => p.LatestNonApprovedBatchStateID == (int)ProjectUpdateStateEnum.Returned),
                    UpdatesApproved = g.Count(p => p.LatestApprovedBatchDate != null && p.LatestApprovedBatchDate >= reportingPeriodStart),
                    RemindersSent = reminders?.Count ?? 0,
                    LastReminderDate = reminders?.LastDate,
                };
            })
            .OrderBy(r => r.PersonName)
            .ToList();

        return grouped;
    }

    public static async Task<EmailContentPreview?> GetEmailContentPreviewAsync(
        WADNRDbContext dbContext, string emailType, string webUrl)
    {
        var config = await dbContext.ProjectUpdateConfigurations
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (config == null)
        {
            return null;
        }

        var introContent = emailType.ToLowerInvariant() switch
        {
            "kickoff" => config.ProjectUpdateKickOffIntroContent,
            "reminder" => config.ProjectUpdateReminderIntroContent,
            "closeout" => config.ProjectUpdateCloseOutIntroContent,
            _ => null
        };

        if (introContent == null)
        {
            introContent = "<em>(No intro content configured)</em>";
        }

        var systemInfo = await dbContext.SystemAttributes
            .AsNoTracking()
            .Select(sa => new
            {
                LogoGuid = sa.SquareLogoFileResource != null ? (Guid?)sa.SquareLogoFileResource.FileResourceGUID : null,
                SupportEmail = sa.PrimaryContactPerson != null ? sa.PrimaryContactPerson.Email : null,
            })
            .FirstOrDefaultAsync();

        var logoUrl = systemInfo?.LogoGuid != null
            ? $"{webUrl}/api/file-resources/{systemInfo.LogoGuid}"
            : "";

        var supportEmail = systemInfo?.SupportEmail ?? "support@wadnr.wa.gov";

        var projectsUrl = $"{webUrl}/my-projects";
        var personName = "<em>Organization Primary Contact</em>";
        var projectList = "<p><em>A list of the recipient's projects that require an update and do not have an update submitted yet will appear here.</em></p>";

        var body = string.Format(EmailTemplate, personName, introContent, projectsUrl, projectList);

        var signature = new StringBuilder();
        signature.Append("<br/>Thank you,<br/>");
        signature.Append("WA DNR Forest Health Tracker team<br/><br/>");
        if (!string.IsNullOrEmpty(logoUrl))
        {
            signature.Append($"<img src=\"{logoUrl}\" width=\"160\" /><br/>");
        }
        signature.Append("<p>");
        signature.Append($"P.S. - You received this email because you are listed as the Primary Contact for these projects. If you feel that you should not be the Primary Contact for one or more of these projects, please <a href=\"mailto:{supportEmail}\">contact support</a>.");
        signature.Append("</p>");

        return new EmailContentPreview
        {
            EmailContentHtml = $"{body}<br/>{signature}"
        };
    }
}
