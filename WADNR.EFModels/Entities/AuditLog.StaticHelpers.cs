using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class AuditLogs
{
    public static async Task<List<ProjectAuditLogGridRow>> ListForProjectAsGridRowAsync(WADNRDbContext dbContext, int projectID)
    {
        var logs = await dbContext.AuditLogs
            .AsNoTracking()
            .Where(a => a.ProjectID == projectID)
            .OrderByDescending(a => a.AuditLogDate)
            .Select(AuditLogProjections.AsProjectGridRow)
            .ToListAsync();

        foreach (var log in logs)
        {
            log.AuditLogEventTypeName = AuditLogEventType.AllLookupDictionary.TryGetValue(log.AuditLogEventTypeID, out var eventType)
                ? eventType.AuditLogEventTypeDisplayName
                : $"Unknown ({log.AuditLogEventTypeID})";
        }

        return logs;
    }
}
