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
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return;

        var submitter = await _dbContext.People.FindAsync(submitterPersonID);
        if (submitter == null) return;

        var approvers = await GetProjectApproversAsync();
        if (!approvers.Any()) return;

        var subject = $"Project Update Submitted: {batch.Project.ProjectName}";
        var body = BuildUpdateSubmittedEmailBody(batch.Project, submitter);

        await SendToRecipientsAsync(approvers, subject, body);
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
        var recipients = GetProjectContactRecipients(batch.Project, submitter);

        if (!recipients.Any()) return;

        var subject = $"Project Update Approved: {batch.Project.ProjectName}";
        var body = BuildUpdateApprovedEmailBody(batch.Project, approver);

        await SendToRecipientsAsync(recipients, subject, body);
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
        var recipients = GetProjectContactRecipients(batch.Project, submitter);

        if (!recipients.Any()) return;

        var subject = $"Project Update Returned: {batch.Project.ProjectName}";
        var body = BuildUpdateReturnedEmailBody(batch.Project, returner, comment);

        await SendToRecipientsAsync(recipients, subject, body);
    }

    #endregion

    #region Create Workflow Notifications

    public async Task SendCreateSubmittedNotificationAsync(int projectID, int submitterPersonID)
    {
        var project = await _dbContext.Projects.FindAsync(projectID);
        if (project == null) return;

        var submitter = await _dbContext.People.FindAsync(submitterPersonID);
        if (submitter == null) return;

        var approvers = await GetProjectApproversAsync();
        if (!approvers.Any()) return;

        var subject = $"New Project Submitted: {project.ProjectName}";
        var body = BuildCreateSubmittedEmailBody(project, submitter);

        await SendToRecipientsAsync(approvers, subject, body);
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
        var recipients = GetProjectContactRecipients(project, proposer);

        if (!recipients.Any()) return;

        var subject = $"New Project Approved: {project.ProjectName}";
        var body = BuildCreateApprovedEmailBody(project, approver);

        await SendToRecipientsAsync(recipients, subject, body);
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
        var recipients = GetProjectContactRecipients(project, proposer);

        if (!recipients.Any()) return;

        var subject = $"New Project Returned: {project.ProjectName}";
        var body = BuildCreateReturnedEmailBody(project, returner, comment);

        await SendToRecipientsAsync(recipients, subject, body);
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
        var recipients = GetProjectContactRecipients(project, proposer);

        if (!recipients.Any()) return;

        var subject = $"New Project Rejected: {project.ProjectName}";
        var body = BuildCreateRejectedEmailBody(project, rejecter, comment);

        await SendToRecipientsAsync(recipients, subject, body);
    }

    #endregion

    #region Helpers

    private async Task SendToRecipientsAsync(List<Person> recipients, string subject, string body)
    {
        foreach (var recipient in recipients)
        {
            if (string.IsNullOrEmpty(recipient.Email)) continue;

            var message = new MailMessage
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(recipient.Email, GetFullName(recipient)));

            try
            {
                await _emailService.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send notification to {recipient.Email}: {ex.Message}");
            }
        }
    }

    private async Task<List<Person>> GetProjectApproversAsync()
    {
        var adminRoleIDs = new[]
        {
            (int)RoleEnum.Admin,
            (int)RoleEnum.EsaAdmin
        };

        return await _dbContext.People
            .Where(p => p.PersonRoles.Any(pr => adminRoleIDs.Contains(pr.RoleID)))
            .Where(p => p.IsActive)
            .Where(p => !string.IsNullOrEmpty(p.Email))
            .ToListAsync();
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
