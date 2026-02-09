using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class SupportRequestLogs
{
    public static async Task<SupportRequestLog> CreateAsync(
        WADNRDbContext dbContext,
        SupportRequestCreate dto,
        Person currentPerson)
    {
        var fullName = $"{currentPerson.FirstName} {currentPerson.LastName}".Trim();

        var supportRequestLog = new SupportRequestLog
        {
            RequestDate = DateTime.UtcNow,
            RequestPersonName = fullName,
            RequestPersonEmail = currentPerson.Email ?? "",
            RequestPersonID = currentPerson.PersonID,
            SupportRequestTypeID = dto.SupportRequestTypeID,
            RequestDescription = dto.RequestDescription,
            RequestPersonOrganization = dto.RequestPersonOrganization,
            RequestPersonPhone = dto.RequestPersonPhone
        };

        dbContext.SupportRequestLogs.Add(supportRequestLog);
        await dbContext.SaveChangesAsync();

        return supportRequestLog;
    }

    public static async Task<List<string>> GetSupportEmailRecipientsAsync(WADNRDbContext dbContext)
    {
        return await dbContext.People
            .AsNoTracking()
            .Where(p => p.ReceiveSupportEmails && p.IsActive && !string.IsNullOrEmpty(p.Email))
            .Select(p => p.Email!)
            .ToListAsync();
    }

    public static MailMessage BuildFeedbackEmail(
        SupportRequestLog supportRequestLog,
        string currentPageUrl,
        string sitkaSupportEmail)
    {
        var subject = $"Support Request for Forest Health Tracker - {DateTime.Now:M/d/yyyy h:mm tt}";

        // Get the support request type display name
        var supportRequestTypeDisplayName = SupportRequestType.AllLookupDictionary.TryGetValue(supportRequestLog.SupportRequestTypeID, out var supportRequestType)
            ? supportRequestType.SupportRequestTypeDisplayName
            : "Unknown";

        var message = $@"
<div style='font-size: 12px; font-family: Arial'>
    <strong>{subject}</strong><br />
    <br />
    <strong>From:</strong> {supportRequestLog.RequestPersonName} - {supportRequestLog.RequestPersonOrganization ?? "(not provided)"}<br />
    <strong>Email:</strong> {supportRequestLog.RequestPersonEmail}<br />
    <strong>Phone:</strong> {supportRequestLog.RequestPersonPhone ?? "(not provided)"}<br />
    <br />
    <strong>Subject:</strong> {supportRequestTypeDisplayName}<br />
    <br />
    <strong>Description:</strong><br />
    {System.Net.WebUtility.HtmlEncode(supportRequestLog.RequestDescription).Replace("\n", "<br />")}
    <br />
    <br />
    <br />
    <div style='font-size: 10px; color: gray'>
    OTHER DETAILS:<br />
    LOGIN: {supportRequestLog.RequestPersonName} (UserID {supportRequestLog.RequestPersonID})<br />
    URL FROM: {currentPageUrl ?? "(not provided)"}<br />
    <br />
    </div>
    <div>You received this email because you are set up as a point of contact for support - if that's not correct, let us know: {sitkaSupportEmail}</div>
</div>";

        var mailMessage = new MailMessage
        {
            Subject = subject,
            Body = message,
            IsBodyHtml = true
        };

        // Reply-To Header
        if (!string.IsNullOrEmpty(supportRequestLog.RequestPersonEmail))
        {
            mailMessage.ReplyToList.Add(supportRequestLog.RequestPersonEmail);
        }

        return mailMessage;
    }
}
