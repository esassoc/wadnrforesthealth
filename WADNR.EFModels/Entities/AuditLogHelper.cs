using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NetTopologySuite.Geometries;

namespace WADNR.EFModels.Entities;

public static class AuditLogHelper
{
    private static readonly HashSet<string> IgnoredTables = new(StringComparer.OrdinalIgnoreCase)
    {
        "AuditLog",
        "FirmaPage",
        "FirmaPageImage",
        "FileResource",
        "Notification",
        "NotificationProject",
        "ProjectExemptReportingYearUpdate",
        "ProjectExternalLinkUpdate",
        "ProjectFundSourceAllocationExpenditureUpdate",
        "ProjectImageUpdate",
        "ProjectNoteUpdate",
        "ProjectLocationStaging",
        "ProjectLocationStagingUpdate",
        "ProjectUpdate",
        "ProjectUpdateBatch",
        "ProjectUpdateHistory",
        "ProjectPerson",
        "SupportRequestLog",
        "ProjectBudgetUpdate",
        "ProjectFundSourceAllocationRequestUpdate",
        "ProjectDocumentUpdate",
        "PersonStewardOrganization",
        "PersonStewardTaxonomyBranch",
        "PersonStewardRegion",
        "SocrataDataMartRawJsonImport",
        "ArcOnlineFinanceApiRawJsonImport",
        "DatabaseMigration"
    };

    public static List<AuditLog> CreateAuditLogsForModifiedOrDeleted(EntityEntry entry, int personID, DateTime changeDate)
    {
        var result = new List<AuditLog>();
        var tableName = entry.Metadata.GetTableName();
        if (tableName == null || IgnoredTables.Contains(tableName))
        {
            return result;
        }

        var primaryKey = GetPrimaryKeyValue(entry);
        if (primaryKey == null)
        {
            return result;
        }

        switch (entry.State)
        {
            case EntityState.Deleted:
                var deletedDescription = $"{tableName}: deleted {GetEntityDisplayName(entry, tableName)}";
                result.Add(CreateAuditLogEntry(entry, tableName, personID, changeDate,
                    AuditLogEventType.Deleted.AuditLogEventTypeID,
                    primaryKey.Value, "*ALL",
                    null, AuditLogEventType.Deleted.AuditLogEventTypeDisplayName, deletedDescription));
                break;

            case EntityState.Modified:
                foreach (var property in entry.Properties)
                {
                    if (!property.IsModified) continue;

                    var propertyName = property.Metadata.Name;
                    if (ShouldSkipProperty(tableName, propertyName, property)) continue;

                    var originalValue = property.OriginalValue;
                    var currentValue = property.CurrentValue;

                    // Skip false-positive modifications where values are effectively the same
                    if (originalValue != null && currentValue != null &&
                        originalValue.ToString() == currentValue.ToString())
                    {
                        continue;
                    }

                    var description = GetAuditDescriptionForProperty(propertyName, originalValue, currentValue, AuditLogEventTypeEnum.Modified);

                    result.Add(CreateAuditLogEntry(entry, tableName, personID, changeDate,
                        AuditLogEventType.Modified.AuditLogEventTypeID,
                        primaryKey.Value, propertyName,
                        originalValue?.ToString(), currentValue?.ToString() ?? string.Empty,
                        description));
                }
                break;
        }

        return result;
    }

    public static List<AuditLog> CreateAuditLogsForAdded(EntityEntry entry, int personID, DateTime changeDate)
    {
        var result = new List<AuditLog>();
        var tableName = entry.Metadata.GetTableName();
        if (tableName == null || IgnoredTables.Contains(tableName))
        {
            return result;
        }

        var primaryKey = GetPrimaryKeyValue(entry);
        if (primaryKey == null)
        {
            return result;
        }

        foreach (var property in entry.Properties)
        {
            var propertyName = property.Metadata.Name;
            if (ShouldSkipProperty(tableName, propertyName, property)) continue;

            var currentValue = property.CurrentValue;

            // Skip null-to-null for Added entries (reduce noise)
            if (currentValue == null) continue;

            var description = GetAuditDescriptionForProperty(propertyName, null, currentValue, AuditLogEventTypeEnum.Added);

            result.Add(CreateAuditLogEntry(entry, tableName, personID, changeDate,
                AuditLogEventType.Added.AuditLogEventTypeID,
                primaryKey.Value, propertyName,
                null, currentValue.ToString() ?? string.Empty,
                description));
        }

        return result;
    }

    private static AuditLog CreateAuditLogEntry(EntityEntry entry, string tableName, int personID,
        DateTime changeDate, int auditLogEventTypeID, int recordID,
        string columnName, string? originalValue, string newValue, string? auditDescription)
    {
        var auditLog = new AuditLog
        {
            PersonID = personID,
            AuditLogDate = changeDate,
            AuditLogEventTypeID = auditLogEventTypeID,
            TableName = tableName,
            RecordID = recordID,
            ColumnName = columnName,
            OriginalValue = originalValue,
            NewValue = newValue,
            AuditDescription = auditDescription
        };

        // Set ProjectID if the entity has one, or if the entity IS a Project
        if (string.Equals(tableName, "Project", StringComparison.OrdinalIgnoreCase))
        {
            auditLog.ProjectID = recordID;
        }
        else
        {
            var projectIDProperty = entry.Metadata.FindProperty("ProjectID");
            if (projectIDProperty != null)
            {
                var projectIDValue = entry.Property("ProjectID").CurrentValue as int?;
                if (projectIDValue.HasValue)
                {
                    auditLog.ProjectID = projectIDValue.Value;
                }
            }
        }

        return auditLog;
    }

    private static int? GetPrimaryKeyValue(EntityEntry entry)
    {
        // First try the IHavePrimaryKey interface (lookup/enum types)
        if (entry.Entity is IHavePrimaryKey havePrimaryKey)
        {
            return havePrimaryKey.PrimaryKey;
        }

        // Fall back to EF Core metadata to find the primary key property
        var primaryKey = entry.Metadata.FindPrimaryKey();
        if (primaryKey == null || primaryKey.Properties.Count != 1)
        {
            return null;
        }

        var pkProperty = primaryKey.Properties[0];
        var pkValue = entry.Property(pkProperty.Name).CurrentValue;
        if (pkValue is int intValue)
        {
            return intValue;
        }

        return null;
    }

    private static bool ShouldSkipProperty(string tableName, string propertyName, PropertyEntry property)
    {
        // Skip PK column (e.g. "ProjectID" for table "Project")
        if (string.Equals(propertyName, $"{tableName}ID", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        // Skip TenantID (legacy convention)
        if (string.Equals(propertyName, "TenantID", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        // Skip geometry/spatial properties (can't meaningfully convert to string)
        var clrType = property.Metadata.ClrType;
        if (typeof(Geometry).IsAssignableFrom(clrType))
        {
            return true;
        }

        return false;
    }

    private static string? GetAuditDescriptionForProperty(string propertyName, object? originalValue, object? currentValue, AuditLogEventTypeEnum eventType)
    {
        switch (propertyName)
        {
            case "ProjectStageID":
                return ResolveLookupDescription("Project Stage",
                    originalValue as int?, currentValue as int?,
                    id => ProjectStage.AllLookupDictionary.TryGetValue(id, out var stage) ? stage.ProjectStageDisplayName : null,
                    eventType);

            case "ProjectImageTimingID":
                return ResolveLookupDescription("Image Timing",
                    originalValue as int?, currentValue as int?,
                    id => ProjectImageTiming.AllLookupDictionary.TryGetValue(id, out var timing) ? timing.ProjectImageTimingDisplayName : null,
                    eventType);

            default:
                return null;
        }
    }

    private static string? ResolveLookupDescription(string fieldName, int? originalID, int? newID,
        Func<int, string?> lookupFunc, AuditLogEventTypeEnum eventType)
    {
        var originalName = originalID.HasValue ? lookupFunc(originalID.Value) ?? "" : "";
        var newName = newID.HasValue ? lookupFunc(newID.Value) ?? "" : "";

        if (string.IsNullOrEmpty(originalName) && string.IsNullOrEmpty(newName))
        {
            return null;
        }

        return eventType switch
        {
            AuditLogEventTypeEnum.Added => $"{fieldName}: set to {newName}",
            AuditLogEventTypeEnum.Modified => $"{fieldName}: {originalName} changed to {newName}",
            AuditLogEventTypeEnum.Deleted => $"{fieldName}: deleted {newName}",
            _ => null
        };
    }

    private static string GetEntityDisplayName(EntityEntry entry, string tableName)
    {
        // Try common display name properties to match legacy IAuditableEntity.AuditDescriptionString
        foreach (var candidateName in new[] { $"{tableName}Name", "Name", "DisplayName" })
        {
            var prop = entry.Metadata.FindProperty(candidateName);
            if (prop != null)
            {
                var value = entry.Property(candidateName).CurrentValue?.ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }
            }
        }

        return $"(ID: {GetPrimaryKeyValue(entry)})";
    }
}
