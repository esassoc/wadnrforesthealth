using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static partial class AuditLogs
{
    public static async Task<List<ProjectAuditLogGridRow>> ListForProjectAsGridRowAsync(WADNRDbContext dbContext, int projectID)
    {
        var technicalColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "CreateDate", "UpdateDate", "CreatePersonID", "UpdatePersonID",
            "ImportedFromTabularData", "ImportedFromGis",
            "CreateGisUploadAttemptID", "UpdateGisUploadAttemptID",
            "FileResourceID",                   // internal FK, not meaningful to users
            "ArcGisObjectID", "ArcGisGlobalID", // GIS internal IDs
            "ImportedFromGisUpload",            // GIS import flag
            "TemporaryTreatmentCacheID"         // internal cache ref
        };

        var logs = await dbContext.AuditLogs
            .AsNoTracking()
            .Where(a => a.ProjectID == projectID)
            .OrderByDescending(a => a.AuditLogDate)
            .Select(AuditLogProjections.AsProjectGridRow)
            .ToListAsync();

        logs = logs.Where(l => !technicalColumns.Contains(l.ColumnName ?? "")).ToList();

        foreach (var log in logs)
        {
            log.AuditLogEventTypeName = AuditLogEventType.AllLookupDictionary.TryGetValue(log.AuditLogEventTypeID, out var eventType)
                ? eventType.AuditLogEventTypeDisplayName
                : $"Unknown ({log.AuditLogEventTypeID})";

            log.Section = PascalCaseToSpaced(log.TableName);

            if (!string.IsNullOrWhiteSpace(log.AuditDescription))
            {
                log.Description = log.AuditDescription;
            }
            else
            {
                log.Description = log.AuditLogEventTypeID switch
                {
                    (int)AuditLogEventTypeEnum.Added => $"{log.ColumnName}: set to {log.NewValue}",
                    (int)AuditLogEventTypeEnum.Modified => $"{log.ColumnName}: {log.OriginalValue} changed to {log.NewValue}",
                    (int)AuditLogEventTypeEnum.Deleted => $"{log.ColumnName}: deleted {log.OriginalValue}",
                    _ => $"{log.ColumnName}: {log.OriginalValue} → {log.NewValue}"
                };
            }
        }

        return logs;
    }

    private static string PascalCaseToSpaced(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return value;
        return PascalCaseRegex().Replace(value, " $1").Trim();
    }

    [GeneratedRegex(@"(?<=[a-z])([A-Z])|(?<=[A-Z])([A-Z][a-z])")]
    private static partial Regex PascalCaseRegex();
}
