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
/// Service for sending notifications related to Project Update workflow state transitions.
/// Matches functionality from legacy ProjectFirma.Web NotificationProject class.
/// </summary>
public interface IProjectUpdateNotificationService
{
    /// <summary>
    /// Send notification when an update is submitted for approval
    /// </summary>
    Task SendSubmittedNotificationAsync(int projectUpdateBatchID, int submitterPersonID);

    /// <summary>
    /// Send notification when an update is approved
    /// </summary>
    Task SendApprovedNotificationAsync(int projectUpdateBatchID, int approverPersonID);

    /// <summary>
    /// Send notification when an update is returned for revisions
    /// </summary>
    Task SendReturnedNotificationAsync(int projectUpdateBatchID, int returnerPersonID, string? comment);
}

public class ProjectUpdateNotificationService : IProjectUpdateNotificationService
{
    private readonly SitkaSmtpClientService _emailService;
    private readonly WADNRDbContext _dbContext;
    private readonly WADNRConfiguration _configuration;

    public ProjectUpdateNotificationService(
        SitkaSmtpClientService emailService,
        WADNRDbContext dbContext,
        IOptions<WADNRConfiguration> configuration)
    {
        _emailService = emailService;
        _dbContext = dbContext;
        _configuration = configuration.Value;
    }

    public async Task SendSubmittedNotificationAsync(int projectUpdateBatchID, int submitterPersonID)
    {
        var batch = await _dbContext.ProjectUpdateBatches
            .Include(b => b.Project)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return;

        var submitter = await _dbContext.People.FindAsync(submitterPersonID);
        if (submitter == null) return;

        // Get approvers/project stewards who should be notified
        var approvers = await GetProjectApproversAsync();

        if (!approvers.Any()) return;

        var subject = $"Project Update Submitted: {batch.Project.ProjectName}";
        var body = BuildSubmittedEmailBody(batch.Project, submitter);

        foreach (var approver in approvers)
        {
            if (string.IsNullOrEmpty(approver.Email)) continue;

            var message = new MailMessage
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(approver.Email, GetFullName(approver)));

            try
            {
                await _emailService.Send(message);
            }
            catch (Exception ex)
            {
                // Log but don't fail the operation
                Console.WriteLine($"Failed to send submission notification to {approver.Email}: {ex.Message}");
            }
        }
    }

    public async Task SendApprovedNotificationAsync(int projectUpdateBatchID, int approverPersonID)
    {
        var batch = await _dbContext.ProjectUpdateBatches
            .Include(b => b.Project)
                .ThenInclude(p => p.ProjectPeople)
                    .ThenInclude(pp => pp.Person)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return;

        var approver = await _dbContext.People.FindAsync(approverPersonID);
        if (approver == null) return;

        // Get the person who last updated (submitted) the batch
        var submitter = await _dbContext.People.FindAsync(batch.LastUpdatePersonID);

        // Get project contacts (primary contact)
        var primaryContact = batch.Project.ProjectPeople
            .FirstOrDefault(pp => pp.ProjectPersonRelationshipTypeID == (int)ProjectPersonRelationshipTypeEnum.PrimaryContact)
            ?.Person;

        var recipients = new List<Person>();
        if (submitter != null) recipients.Add(submitter);
        if (primaryContact != null && primaryContact.PersonID != submitter?.PersonID)
        {
            recipients.Add(primaryContact);
        }

        if (!recipients.Any()) return;

        var subject = $"Project Update Approved: {batch.Project.ProjectName}";
        var body = BuildApprovedEmailBody(batch.Project, approver);

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
                Console.WriteLine($"Failed to send approval notification to {recipient.Email}: {ex.Message}");
            }
        }
    }

    public async Task SendReturnedNotificationAsync(int projectUpdateBatchID, int returnerPersonID, string? comment)
    {
        var batch = await _dbContext.ProjectUpdateBatches
            .Include(b => b.Project)
                .ThenInclude(p => p.ProjectPeople)
                    .ThenInclude(pp => pp.Person)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return;

        var returner = await _dbContext.People.FindAsync(returnerPersonID);
        if (returner == null) return;

        // Get the person who last updated (submitted) the batch
        var submitter = await _dbContext.People.FindAsync(batch.LastUpdatePersonID);

        // Get project contacts (primary contact)
        var primaryContact = batch.Project.ProjectPeople
            .FirstOrDefault(pp => pp.ProjectPersonRelationshipTypeID == (int)ProjectPersonRelationshipTypeEnum.PrimaryContact)
            ?.Person;

        var recipients = new List<Person>();
        if (submitter != null) recipients.Add(submitter);
        if (primaryContact != null && primaryContact.PersonID != submitter?.PersonID)
        {
            recipients.Add(primaryContact);
        }

        if (!recipients.Any()) return;

        var subject = $"Project Update Returned: {batch.Project.ProjectName}";
        var body = BuildReturnedEmailBody(batch.Project, returner, comment);

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
                Console.WriteLine($"Failed to send return notification to {recipient.Email}: {ex.Message}");
            }
        }
    }

    private async Task<List<Person>> GetProjectApproversAsync()
    {
        // Get users with admin roles through PersonRoles
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

    private string BuildSubmittedEmailBody(Project project, Person submitter)
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

    private string BuildApprovedEmailBody(Project project, Person approver)
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

    private string BuildReturnedEmailBody(Project project, Person returner, string? comment)
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

    private static string GetFullName(Person person)
    {
        return $"{person.FirstName} {person.LastName}".Trim();
    }
}
