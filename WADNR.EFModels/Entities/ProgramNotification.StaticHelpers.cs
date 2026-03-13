using System.Net.Mail;
using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

public static class ProgramNotifications
{
    /// <summary>
    /// Processes all program notification configurations and sends reminder emails
    /// for completed projects that are past their maintenance recurrence interval.
    /// Returns the list of ProgramNotificationSent records created.
    /// </summary>
    public static async Task<List<ProgramNotificationSent>> ProcessRemindersAsync(
        WADNRDbContext dbContext, string contactSupportEmail, string toolDisplayName, string webUrl)
    {
        var allNotificationsSent = new List<ProgramNotificationSent>();

        var configs = await dbContext.ProgramNotificationConfigurations
            .Include(c => c.Program)
                .ThenInclude(p => p.ProjectPrograms)
                    .ThenInclude(pp => pp.Project)
                        .ThenInclude(proj => proj.ProjectPeople)
                            .ThenInclude(pp => pp.Person)
            .Include(c => c.Program)
                .ThenInclude(p => p.ProjectPrograms)
                    .ThenInclude(pp => pp.Project)
                        .ThenInclude(proj => proj.ProgramNotificationSentProjects)
                            .ThenInclude(pnsp => pnsp.ProgramNotificationSent)
            .ToListAsync();

        foreach (var config in configs)
        {
            if (config.ProgramNotificationTypeID !=
                (int)ProgramNotificationTypeEnum.CompletedProjectsMaintenanceReminder)
            {
                continue;
            }

            var notifications = ProcessCompletedProjectsMaintenanceReminder(config, contactSupportEmail,
                toolDisplayName, webUrl);
            allNotificationsSent.AddRange(notifications);
        }

        if (allNotificationsSent.Count > 0)
        {
            dbContext.ProgramNotificationSents.AddRange(allNotificationsSent);
            await dbContext.SaveChangesWithNoAuditingAsync();
        }

        return allNotificationsSent;
    }

    private static List<ProgramNotificationSent> ProcessCompletedProjectsMaintenanceReminder(
        ProgramNotificationConfiguration config, string contactSupportEmail, string toolDisplayName, string webUrl)
    {
        var recurrenceIntervalInYears =
            RecurrenceInterval.AllLookupDictionary[config.RecurrenceIntervalID].RecurrenceIntervalInYears;

        var allProjects = config.Program.ProjectPrograms.Select(pp => pp.Project).ToList();
        var completedProjects = allProjects
            .Where(p => p.ProjectStageID == (int)ProjectStageEnum.Completed)
            .Where(p => p.CompletionDate.HasValue &&
                        p.CompletionDate.Value.AddYears(recurrenceIntervalInYears) < DateOnly.FromDateTime(DateTime.Now))
            .ToList();

        var projectsNeedingNotification = new List<Project>();
        foreach (var project in completedProjects)
        {
            if (!project.ProgramNotificationSentProjects.Any())
            {
                projectsNeedingNotification.Add(project);
                continue;
            }

            var lastNotificationDate = project.ProgramNotificationSentProjects
                .Select(x => x.ProgramNotificationSent)
                .OrderByDescending(pns => pns.ProgramNotificationSentDate)
                .First()
                .ProgramNotificationSentDate;

            if (lastNotificationDate.AddYears(recurrenceIntervalInYears) < DateTime.Now)
            {
                projectsNeedingNotification.Add(project);
            }
        }

        return BuildNotifications(projectsNeedingNotification, config, contactSupportEmail, toolDisplayName, webUrl);
    }

    private static List<ProgramNotificationSent> BuildNotifications(List<Project> projects,
        ProgramNotificationConfiguration config, string contactSupportEmail, string toolDisplayName, string webUrl)
    {
        var notifications = new List<ProgramNotificationSent>();

        const int primaryContactTypeID = (int)ProjectPersonRelationshipTypeEnum.PrimaryContact;
        const int privateLandownerTypeID = (int)ProjectPersonRelationshipTypeEnum.PrivateLandowner;
        const string reminderSubject = "Forest Health Tracker - Time to update your Project";

        // Group projects by primary contact
        var projectsWithPrimaryContact = projects
            .Where(p => p.ProjectPeople.Any(pp =>
                pp.ProjectPersonRelationshipTypeID == primaryContactTypeID &&
                !string.IsNullOrEmpty(pp.Person.Email)))
            .ToList();

        var groupedByPrimaryContact = projectsWithPrimaryContact
            .GroupBy(p => p.ProjectPeople
                .First(pp => pp.ProjectPersonRelationshipTypeID == primaryContactTypeID)
                .Person)
            .ToList();

        foreach (var group in groupedByPrimaryContact)
        {
            var person = group.Key;
            var personProjects = group.ToList();
            if (personProjects.Count == 0 || string.IsNullOrEmpty(person.Email)) continue;

            var mailMessage = GenerateReminderEmail(person, personProjects, config.NotificationEmailText,
                reminderSubject, contactSupportEmail, toolDisplayName, webUrl);

            var notificationSent = CreateNotificationSent(config, person, personProjects, mailMessage);
            notifications.Add(notificationSent);
        }

        // Group projects by private landowner contacts
        var privateLandownerProjectPeople = projects
            .SelectMany(p => p.ProjectPeople)
            .Where(pp => pp.ProjectPersonRelationshipTypeID == privateLandownerTypeID &&
                         !string.IsNullOrEmpty(pp.Person.Email))
            .ToList();

        var groupedByPrivateLandowner = privateLandownerProjectPeople
            .GroupBy(pp => pp.Person, pp => pp.Project)
            .ToList();

        foreach (var group in groupedByPrivateLandowner)
        {
            var person = group.Key;
            var personProjects = group.ToList();
            if (personProjects.Count == 0 || string.IsNullOrEmpty(person.Email)) continue;

            var mailMessage = GenerateReminderEmail(person, personProjects, config.NotificationEmailText,
                reminderSubject, contactSupportEmail, toolDisplayName, webUrl);

            var notificationSent = CreateNotificationSent(config, person, personProjects, mailMessage);
            notifications.Add(notificationSent);
        }

        return notifications;
    }

    private static ProgramNotificationSent CreateNotificationSent(ProgramNotificationConfiguration config,
        Person person, List<Project> projects, MailMessage mailMessage)
    {
        var notificationSent = new ProgramNotificationSent
        {
            ProgramNotificationConfigurationID = config.ProgramNotificationConfigurationID,
            SentToPersonID = person.PersonID,
            ProgramNotificationSentDate = DateTime.Now
        };

        foreach (var project in projects)
        {
            notificationSent.ProgramNotificationSentProjects.Add(new ProgramNotificationSentProject
            {
                ProjectID = project.ProjectID
            });
        }

        return notificationSent;
    }

    public static MailMessage GenerateReminderEmail(Person person, List<Project> projects,
        string introContent, string subject, string contactSupportEmail, string toolDisplayName, string webUrl)
    {
        var fullName = $"{person.FirstName} {person.LastName}";
        var projectLinks = projects
            .OrderBy(p => p.ProjectName)
            .Select(p =>
            {
                var projectUrl = $"{webUrl}/projects/{p.ProjectID}";
                return $@"<div style=""font-size:smaller""><a href=""{projectUrl}"">{p.ProjectName}</a></div>";
            });

        var body = $@"Hello {fullName},<br/><br/>
{introContent}
<div style=""font-weight:bold"">Your projects that require an update are:</div>
<div style=""margin-left: 15px"">
    {string.Join("<br/>", projectLinks)}
</div>
<br/>
Thank you,<br />
{toolDisplayName} team<br/><br/>
<p>
P.S. - You received this email because you are listed as a Contact for these projects. If you feel that you should not be a Contact for one or more of these projects, please <a href=""mailto:{contactSupportEmail}"">contact support</a>.
</p>";

        return new MailMessage
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
    }
}
