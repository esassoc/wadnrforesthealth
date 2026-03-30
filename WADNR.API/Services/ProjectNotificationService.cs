using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WADNR.Common.EMail;
using WADNR.EFModels.Entities;

namespace WADNR.API.Services;

/// <summary>
/// Service for sending notifications related to Project workflow state transitions.
/// Handles both create-workflow and update-workflow notifications.
/// Matches functionality from legacy ProjectFirma.Web NotificationProject class.
/// </summary>
public class ProjectNotificationService
{
    private readonly SitkaSmtpClientService _emailService;
    private readonly WADNRDbContext _dbContext;
    private readonly WADNRConfiguration _configuration;

    public ProjectNotificationService(
        SitkaSmtpClientService emailService,
        WADNRDbContext dbContext,
        IOptions<WADNRConfiguration> configuration)
    {
        _emailService = emailService;
        _dbContext = dbContext;
        _configuration = configuration.Value;
    }

    #region Update Workflow Notifications

    public async Task SendUpdateSubmittedNotificationAsync(int projectUpdateBatchID, int submitterPersonID)
    {
        var batch = await _dbContext.ProjectUpdateBatches
            .Include(b => b.Project)
                .ThenInclude(p => p.ProjectPeople)
                    .ThenInclude(pp => pp.Person)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return;

        var submitter = await _dbContext.People.FindAsync(submitterPersonID);
        if (submitter == null) return;

        var stewardPeople = await GetNotificationRecipientsAsync(batch.ProjectID);
        if (!stewardPeople.Any()) return;

        var projectContacts = GetProjectContactRecipients(batch.Project, submitter);
        await PersistNotificationsAsync(batch.ProjectID, NotificationTypeEnum.ProjectUpdateSubmitted, projectContacts);

        var subject = $"Project Update Submitted: {batch.Project.ProjectName}";
        var body = BuildUpdateSubmittedEmailBody(batch.Project, submitter);

        await SendNotificationEmailAsync(subject, body, toRecipients: stewardPeople, replyToRecipients: projectContacts);
    }

    public async Task SendUpdateApprovedNotificationAsync(int projectUpdateBatchID, int approverPersonID)
    {
        var batch = await _dbContext.ProjectUpdateBatches
            .Include(b => b.Project)
                .ThenInclude(p => p.ProjectPeople)
                    .ThenInclude(pp => pp.Person)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return;

        var approver = await _dbContext.People.FindAsync(approverPersonID);
        if (approver == null) return;

        var submitter = await _dbContext.People.FindAsync(batch.LastUpdatePersonID);
        var projectContacts = GetProjectContactRecipients(batch.Project, submitter);

        if (!projectContacts.Any()) return;

        await PersistNotificationsAsync(batch.ProjectID, NotificationTypeEnum.ProjectUpdateApproved, projectContacts);

        var stewardPeople = await GetNotificationRecipientsAsync(batch.ProjectID);

        var subject = $"Project Update Approved: {batch.Project.ProjectName}";
        var body = BuildUpdateApprovedEmailBody(batch.Project, approver);

        await SendNotificationEmailAsync(subject, body, toRecipients: projectContacts, ccRecipients: stewardPeople, replyToRecipients: new List<Person> { approver });
    }

    public async Task SendUpdateReturnedNotificationAsync(int projectUpdateBatchID, int returnerPersonID, string? comment)
    {
        var batch = await _dbContext.ProjectUpdateBatches
            .Include(b => b.Project)
                .ThenInclude(p => p.ProjectPeople)
                    .ThenInclude(pp => pp.Person)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return;

        var returner = await _dbContext.People.FindAsync(returnerPersonID);
        if (returner == null) return;

        var submitter = await _dbContext.People.FindAsync(batch.LastUpdatePersonID);
        var projectContacts = GetProjectContactRecipients(batch.Project, submitter);

        if (!projectContacts.Any()) return;

        await PersistNotificationsAsync(batch.ProjectID, NotificationTypeEnum.ProjectUpdateReturned, projectContacts);

        var stewardPeople = await GetNotificationRecipientsAsync(batch.ProjectID);

        var subject = $"Project Update Returned: {batch.Project.ProjectName}";
        var body = BuildUpdateReturnedEmailBody(batch.Project, returner, comment);

        await SendNotificationEmailAsync(subject, body, toRecipients: projectContacts, ccRecipients: stewardPeople, replyToRecipients: new List<Person> { returner });
    }

    #endregion

    #region Create Workflow Notifications

    public async Task SendCreateSubmittedNotificationAsync(int projectID, int submitterPersonID)
    {
        var project = await _dbContext.Projects
            .Include(p => p.ProjectPeople)
                .ThenInclude(pp => pp.Person)
            .FirstOrDefaultAsync(p => p.ProjectID == projectID);
        if (project == null) return;

        var submitter = await _dbContext.People.FindAsync(submitterPersonID);
        if (submitter == null) return;

        var stewardPeople = await GetNotificationRecipientsAsync(projectID);
        if (!stewardPeople.Any()) return;

        var projectContacts = GetProjectContactRecipients(project, submitter);
        await PersistNotificationsAsync(projectID, NotificationTypeEnum.ProjectSubmitted, projectContacts);

        var subject = $"New Project Submitted: {project.ProjectName}";
        var body = BuildCreateSubmittedEmailBody(project, submitter);

        await SendNotificationEmailAsync(subject, body, toRecipients: stewardPeople, replyToRecipients: projectContacts);
    }

    public async Task SendCreateApprovedNotificationAsync(int projectID, int approverPersonID)
    {
        var project = await _dbContext.Projects
            .Include(p => p.ProjectPeople)
                .ThenInclude(pp => pp.Person)
            .FirstOrDefaultAsync(p => p.ProjectID == projectID);

        if (project == null) return;

        var approver = await _dbContext.People.FindAsync(approverPersonID);
        if (approver == null) return;

        var proposer = project.ProposingPersonID.HasValue
            ? await _dbContext.People.FindAsync(project.ProposingPersonID.Value)
            : null;
        var projectContacts = GetProjectContactRecipients(project, proposer);

        if (!projectContacts.Any()) return;

        await PersistNotificationsAsync(projectID, NotificationTypeEnum.ProjectApproved, projectContacts);

        var stewardPeople = await GetNotificationRecipientsAsync(projectID);

        var subject = $"New Project Approved: {project.ProjectName}";
        var body = BuildCreateApprovedEmailBody(project, approver);

        await SendNotificationEmailAsync(subject, body, toRecipients: projectContacts, ccRecipients: stewardPeople, replyToRecipients: new List<Person> { approver });
    }

    public async Task SendCreateReturnedNotificationAsync(int projectID, int returnerPersonID, string? comment)
    {
        var project = await _dbContext.Projects
            .Include(p => p.ProjectPeople)
                .ThenInclude(pp => pp.Person)
            .FirstOrDefaultAsync(p => p.ProjectID == projectID);

        if (project == null) return;

        var returner = await _dbContext.People.FindAsync(returnerPersonID);
        if (returner == null) return;

        var proposer = project.ProposingPersonID.HasValue
            ? await _dbContext.People.FindAsync(project.ProposingPersonID.Value)
            : null;
        var projectContacts = GetProjectContactRecipients(project, proposer);

        if (!projectContacts.Any()) return;

        await PersistNotificationsAsync(projectID, NotificationTypeEnum.ProjectReturned, projectContacts);

        var stewardPeople = await GetNotificationRecipientsAsync(projectID);

        var subject = $"New Project Returned: {project.ProjectName}";
        var body = BuildCreateReturnedEmailBody(project, returner, comment);

        await SendNotificationEmailAsync(subject, body, toRecipients: projectContacts, ccRecipients: stewardPeople, replyToRecipients: new List<Person> { returner });
    }

    public async Task SendCreateRejectedNotificationAsync(int projectID, int rejecterPersonID, string? comment)
    {
        var project = await _dbContext.Projects
            .Include(p => p.ProjectPeople)
                .ThenInclude(pp => pp.Person)
            .FirstOrDefaultAsync(p => p.ProjectID == projectID);

        if (project == null) return;

        var rejecter = await _dbContext.People.FindAsync(rejecterPersonID);
        if (rejecter == null) return;

        var proposer = project.ProposingPersonID.HasValue
            ? await _dbContext.People.FindAsync(project.ProposingPersonID.Value)
            : null;
        var projectContacts = GetProjectContactRecipients(project, proposer);

        if (!projectContacts.Any()) return;

        var subject = $"New Project Rejected: {project.ProjectName}";
        var body = BuildCreateRejectedEmailBody(project, rejecter, comment);

        await SendNotificationEmailAsync(subject, body, toRecipients: projectContacts, replyToRecipients: new List<Person> { rejecter });
    }

    #endregion

    #region Helpers

    private async Task PersistNotificationsAsync(int projectID, NotificationTypeEnum notificationType, List<Person> recipients)
    {
        var notificationDate = DateTime.Now;

        foreach (var recipient in recipients)
        {
            var notification = new Notification
            {
                NotificationTypeID = (int)notificationType,
                PersonID = recipient.PersonID,
                NotificationDate = notificationDate,
                NotificationProjects = new List<NotificationProject>
                {
                    new NotificationProject { ProjectID = projectID }
                }
            };
            _dbContext.Notifications.Add(notification);
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task SendNotificationEmailAsync(
        string subject,
        string body,
        List<Person> toRecipients,
        List<Person>? ccRecipients = null,
        List<Person>? replyToRecipients = null)
    {
        var message = new MailMessage
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        foreach (var person in toRecipients.Where(p => !string.IsNullOrEmpty(p.Email)))
        {
            message.To.Add(new MailAddress(person.Email, GetFullName(person)));
        }

        if (ccRecipients != null)
        {
            foreach (var person in ccRecipients.Where(p => !string.IsNullOrEmpty(p.Email)))
            {
                message.CC.Add(new MailAddress(person.Email, GetFullName(person)));
            }
        }

        if (replyToRecipients != null)
        {
            foreach (var person in replyToRecipients.Where(p => !string.IsNullOrEmpty(p.Email)))
            {
                message.ReplyToList.Add(new MailAddress(person.Email, GetFullName(person)));
            }
        }

        if (!message.To.Any()) return;

        try
        {
            await _emailService.Send(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send notification: {ex.Message}");
        }
    }

    /// <summary>
    /// Matches legacy GetProjectStewardPeople: people who opted in to receive notifications
    /// (ReceiveSupportEmails) UNION people in the project's stewardship org with ProjectSteward role.
    /// </summary>
    private async Task<List<Person>> GetNotificationRecipientsAsync(int projectID)
    {
        var notificationPeople = await _dbContext.People
            .Where(p => p.ReceiveSupportEmails && p.IsActive && !string.IsNullOrEmpty(p.Email))
            .ToListAsync();

        var stewardOrgIDs = await _dbContext.ProjectOrganizations
            .Where(po => po.ProjectID == projectID && po.RelationshipType.CanStewardProjects)
            .Select(po => po.OrganizationID)
            .ToListAsync();

        if (stewardOrgIDs.Any())
        {
            var projectStewards = await _dbContext.People
                .Where(p => p.OrganizationID.HasValue && stewardOrgIDs.Contains(p.OrganizationID.Value))
                .Where(p => p.PersonRoles.Any(pr => pr.RoleID == (int)RoleEnum.ProjectSteward))
                .Where(p => p.IsActive && !string.IsNullOrEmpty(p.Email))
                .ToListAsync();

            notificationPeople = notificationPeople
                .Union(projectStewards)
                .DistinctBy(p => p.PersonID)
                .ToList();
        }

        return notificationPeople;
    }

    private static List<Person> GetProjectContactRecipients(Project project, Person? primaryRecipient)
    {
        var primaryContact = project.ProjectPeople
            .FirstOrDefault(pp => pp.ProjectPersonRelationshipTypeID == (int)ProjectPersonRelationshipTypeEnum.PrimaryContact)
            ?.Person;

        var recipients = new List<Person>();
        if (primaryRecipient != null) recipients.Add(primaryRecipient);
        if (primaryContact != null && primaryContact.PersonID != primaryRecipient?.PersonID)
        {
            recipients.Add(primaryContact);
        }
        return recipients;
    }

    #endregion

    #region Update Workflow Email Bodies

    private string BuildUpdateSubmittedEmailBody(Project project, Person submitter)
    {
        var projectUrl = $"{_configuration.WebUrl}/projects/{project.ProjectID}";
        var updateUrl = $"{_configuration.WebUrl}/projects/{project.ProjectID}/update";

        return $@"
<html>
<body>
<p>Hello,</p>

<p>A project update has been submitted for your review.</p>

<p><strong>Project:</strong> <a href=""{projectUrl}"">{System.Net.WebUtility.HtmlEncode(project.ProjectName)}</a></p>
<p><strong>Submitted by:</strong> {System.Net.WebUtility.HtmlEncode(GetFullName(submitter))} ({System.Net.WebUtility.HtmlEncode(submitter.Email ?? "")})</p>
<p><strong>Submitted on:</strong> {DateTime.Now:MMMM d, yyyy}</p>

<p>Please <a href=""{updateUrl}"">review the update</a> and approve or return it for revisions.</p>

<p>Thank you,<br/>
WA DNR Forest Health Tracker</p>
</body>
</html>";
    }

    private string BuildUpdateApprovedEmailBody(Project project, Person approver)
    {
        var projectUrl = $"{_configuration.WebUrl}/projects/{project.ProjectID}";

        return $@"
<html>
<body>
<p>Hello,</p>

<p>Good news! Your project update has been approved and the changes are now visible publicly.</p>

<p><strong>Project:</strong> <a href=""{projectUrl}"">{System.Net.WebUtility.HtmlEncode(project.ProjectName)}</a></p>
<p><strong>Approved by:</strong> {System.Net.WebUtility.HtmlEncode(GetFullName(approver))}</p>
<p><strong>Approved on:</strong> {DateTime.Now:MMMM d, yyyy}</p>

<p>No further action is required.</p>

<p>Thank you,<br/>
WA DNR Forest Health Tracker</p>
</body>
</html>";
    }

    private string BuildUpdateReturnedEmailBody(Project project, Person returner, string? comment)
    {
        var updateUrl = $"{_configuration.WebUrl}/projects/{project.ProjectID}/update";

        var commentHtml = string.IsNullOrEmpty(comment)
            ? ""
            : $@"<p><strong>Comments from reviewer:</strong></p>
<blockquote style=""border-left: 3px solid #ccc; padding-left: 10px; margin-left: 0;"">{System.Net.WebUtility.HtmlEncode(comment)}</blockquote>";

        return $@"
<html>
<body>
<p>Hello,</p>

<p>Your project update has been returned and requires revisions before it can be approved.</p>

<p><strong>Project:</strong> {System.Net.WebUtility.HtmlEncode(project.ProjectName)}</p>
<p><strong>Returned by:</strong> {System.Net.WebUtility.HtmlEncode(GetFullName(returner))} (<a href=""mailto:{System.Net.WebUtility.HtmlEncode(returner.Email ?? "")}"">{System.Net.WebUtility.HtmlEncode(returner.Email ?? "")}</a>)</p>
<p><strong>Returned on:</strong> {DateTime.Now:MMMM d, yyyy}</p>

{commentHtml}

<p>Please <a href=""{updateUrl}"">review and update your submission</a>, then resubmit for approval.</p>

<p>If you have questions, please contact the reviewer directly.</p>

<p>Thank you,<br/>
WA DNR Forest Health Tracker</p>
</body>
</html>";
    }

    #endregion

    #region Create Workflow Email Bodies

    private string BuildCreateSubmittedEmailBody(Project project, Person submitter)
    {
        var projectUrl = $"{_configuration.WebUrl}/projects/{project.ProjectID}";

        return $@"
<html>
<body>
<p>Hello,</p>

<p>A new project has been submitted for your review.</p>

<p><strong>Project:</strong> <a href=""{projectUrl}"">{System.Net.WebUtility.HtmlEncode(project.ProjectName)}</a></p>
<p><strong>Submitted by:</strong> {System.Net.WebUtility.HtmlEncode(GetFullName(submitter))} ({System.Net.WebUtility.HtmlEncode(submitter.Email ?? "")})</p>
<p><strong>Submitted on:</strong> {DateTime.Now:MMMM d, yyyy}</p>

<p>Please <a href=""{projectUrl}"">review the project</a> and approve, return, or reject it.</p>

<p>Thank you,<br/>
WA DNR Forest Health Tracker</p>
</body>
</html>";
    }

    private string BuildCreateApprovedEmailBody(Project project, Person approver)
    {
        var projectUrl = $"{_configuration.WebUrl}/projects/{project.ProjectID}";

        return $@"
<html>
<body>
<p>Hello,</p>

<p>Good news! Your new project has been approved.</p>

<p><strong>Project:</strong> <a href=""{projectUrl}"">{System.Net.WebUtility.HtmlEncode(project.ProjectName)}</a></p>
<p><strong>Approved by:</strong> {System.Net.WebUtility.HtmlEncode(GetFullName(approver))}</p>
<p><strong>Approved on:</strong> {DateTime.Now:MMMM d, yyyy}</p>

<p>No further action is required.</p>

<p>Thank you,<br/>
WA DNR Forest Health Tracker</p>
</body>
</html>";
    }

    private string BuildCreateReturnedEmailBody(Project project, Person returner, string? comment)
    {
        var projectUrl = $"{_configuration.WebUrl}/projects/{project.ProjectID}/create";

        var commentHtml = string.IsNullOrEmpty(comment)
            ? ""
            : $@"<p><strong>Comments from reviewer:</strong></p>
<blockquote style=""border-left: 3px solid #ccc; padding-left: 10px; margin-left: 0;"">{System.Net.WebUtility.HtmlEncode(comment)}</blockquote>";

        return $@"
<html>
<body>
<p>Hello,</p>

<p>Your new project has been returned and requires revisions before it can be approved.</p>

<p><strong>Project:</strong> {System.Net.WebUtility.HtmlEncode(project.ProjectName)}</p>
<p><strong>Returned by:</strong> {System.Net.WebUtility.HtmlEncode(GetFullName(returner))} (<a href=""mailto:{System.Net.WebUtility.HtmlEncode(returner.Email ?? "")}"">{System.Net.WebUtility.HtmlEncode(returner.Email ?? "")}</a>)</p>
<p><strong>Returned on:</strong> {DateTime.Now:MMMM d, yyyy}</p>

{commentHtml}

<p>Please <a href=""{projectUrl}"">review and update your project</a>, then resubmit for approval.</p>

<p>If you have questions, please contact the reviewer directly.</p>

<p>Thank you,<br/>
WA DNR Forest Health Tracker</p>
</body>
</html>";
    }

    private string BuildCreateRejectedEmailBody(Project project, Person rejecter, string? comment)
    {
        var commentHtml = string.IsNullOrEmpty(comment)
            ? ""
            : $@"<p><strong>Comments from reviewer:</strong></p>
<blockquote style=""border-left: 3px solid #ccc; padding-left: 10px; margin-left: 0;"">{System.Net.WebUtility.HtmlEncode(comment)}</blockquote>";

        return $@"
<html>
<body>
<p>Hello,</p>

<p>Unfortunately, your new project has been rejected.</p>

<p><strong>Project:</strong> {System.Net.WebUtility.HtmlEncode(project.ProjectName)}</p>
<p><strong>Rejected by:</strong> {System.Net.WebUtility.HtmlEncode(GetFullName(rejecter))} (<a href=""mailto:{System.Net.WebUtility.HtmlEncode(rejecter.Email ?? "")}"">{System.Net.WebUtility.HtmlEncode(rejecter.Email ?? "")}</a>)</p>
<p><strong>Rejected on:</strong> {DateTime.Now:MMMM d, yyyy}</p>

{commentHtml}

<p>If you have questions, please contact the reviewer directly.</p>

<p>Thank you,<br/>
WA DNR Forest Health Tracker</p>
</body>
</html>";
    }

    #endregion

    private static string GetFullName(Person person)
    {
        return $"{person.FirstName} {person.LastName}".Trim();
    }
}
