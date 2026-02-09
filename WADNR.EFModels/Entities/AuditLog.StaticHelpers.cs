using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class AuditLogs
{
    public static async Task<List<ProjectAuditLogGridRow>> ListForProjectAsGridRowAsync(WADNRDbContext dbContext, int projectID)
    {
        var rawLogs = await dbContext.AuditLogs
            .AsNoTracking()
            .Where(a => a.ProjectID == projectID)
            .Select(a => new
            {
                a.AuditLogID,
                a.AuditLogDate,
                a.AuditLogEventTypeID,
                a.TableName,
                a.ColumnName,
                a.OriginalValue,
                a.NewValue,
                a.AuditDescription,
                PersonFirstName = a.Person.FirstName,
                PersonLastName = a.Person.LastName
            })
            .OrderByDescending(a => a.AuditLogDate)
            .ToListAsync();

        var logs = rawLogs
            .Select(a => new ProjectAuditLogGridRow
            {
                AuditLogID = a.AuditLogID,
                AuditLogDate = a.AuditLogDate,
                PersonName = $"{a.PersonFirstName} {a.PersonLastName}",
                AuditLogEventTypeName = AuditLogEventType.AllLookupDictionary.TryGetValue(a.AuditLogEventTypeID, out var eventType)
                    ? eventType.AuditLogEventTypeDisplayName
                    : $"Unknown ({a.AuditLogEventTypeID})",
                TableName = a.TableName,
                ColumnName = a.ColumnName,
                OriginalValue = a.OriginalValue,
                NewValue = a.NewValue,
                AuditDescription = a.AuditDescription
            })
            .ToList();

        return logs;
    }
}
