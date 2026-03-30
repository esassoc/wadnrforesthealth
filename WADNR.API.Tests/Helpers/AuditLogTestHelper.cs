using Microsoft.EntityFrameworkCore;
using WADNR.EFModels.Entities;

namespace WADNR.API.Tests.Helpers;

/// <summary>
/// Helper methods for querying and managing audit logs during tests.
/// </summary>
public static class AuditLogTestHelper
{
    /// <summary>
    /// Gets all audit logs for a specific project.
    /// </summary>
    public static async Task<List<AuditLog>> GetAuditLogsForProjectAsync(
        WADNRDbContext dbContext,
        int projectID,
        DateTime? sinceDate = null)
    {
        // Clear change tracker to ensure fresh data from database
        dbContext.ChangeTracker.Clear();

        var query = dbContext.AuditLogs.AsNoTracking().Where(al => al.ProjectID.HasValue && al.ProjectID.Value == projectID);

        if (sinceDate.HasValue)
        {
            query = query.Where(al => al.AuditLogDate >= sinceDate);
        }

        return await query.OrderBy(al => al.AuditLogDate).ThenBy(al => al.AuditLogID).ToListAsync();
    }

    /// <summary>
    /// Gets audit logs for a specific project and table.
    /// </summary>
    public static async Task<List<AuditLog>> GetAuditLogsForTableAsync(
        WADNRDbContext dbContext,
        int projectID,
        string tableName,
        DateTime? sinceDate = null)
    {
        // Clear change tracker to ensure fresh data from database
        dbContext.ChangeTracker.Clear();

        var query = dbContext.AuditLogs.AsNoTracking()
            .Where(al => al.ProjectID.HasValue && al.ProjectID.Value == projectID && al.TableName == tableName);

        if (sinceDate.HasValue)
        {
            query = query.Where(al => al.AuditLogDate >= sinceDate);
        }

        return await query.OrderBy(al => al.AuditLogDate).ThenBy(al => al.AuditLogID).ToListAsync();
    }

    /// <summary>
    /// Gets audit logs filtered by event type (Added=1, Deleted=2, Modified=3).
    /// </summary>
    public static async Task<List<AuditLog>> GetAuditLogsForTableAndEventTypeAsync(
        WADNRDbContext dbContext,
        int projectID,
        string tableName,
        int auditLogEventTypeID,
        DateTime? sinceDate = null)
    {
        // Use a fresh DbContext to avoid any caching issues
        using var freshContext = AssemblySteps.CreateFreshDbContext();

        var query = freshContext.AuditLogs.AsNoTracking()
            .Where(al => al.ProjectID.HasValue && al.ProjectID.Value == projectID &&
                         al.TableName == tableName &&
                         al.AuditLogEventTypeID == auditLogEventTypeID);

        if (sinceDate.HasValue)
        {
            query = query.Where(al => al.AuditLogDate >= sinceDate);
        }

        return await query.OrderBy(al => al.AuditLogDate).ThenBy(al => al.AuditLogID).ToListAsync();
    }

    /// <summary>
    /// Clears all audit logs for a specific project.
    /// </summary>
    public static async Task ClearAuditLogsForProjectAsync(
        WADNRDbContext dbContext,
        int projectID)
    {
        await dbContext.AuditLogs
            .Where(al => al.ProjectID == projectID)
            .ExecuteDeleteAsync();
    }

    /// <summary>
    /// Gets the count of audit logs for a project since a given date.
    /// </summary>
    public static async Task<int> GetAuditLogCountAsync(
        WADNRDbContext dbContext,
        int projectID,
        DateTime? sinceDate = null)
    {
        var query = dbContext.AuditLogs.Where(al => al.ProjectID == projectID);

        if (sinceDate.HasValue)
        {
            query = query.Where(al => al.AuditLogDate >= sinceDate);
        }

        return await query.CountAsync();
    }
}
