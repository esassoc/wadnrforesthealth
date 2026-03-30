using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace WADNR.EFModels.Entities;

public partial class WADNRDbContext
{
    private readonly IAuditUserProvider? _auditUserProvider;
    private bool _suppressAuditLogging;

    public WADNRDbContext(DbContextOptions<WADNRDbContext> options, IAuditUserProvider auditUserProvider)
        : base(options)
    {
        _auditUserProvider = auditUserProvider;
    }

    public override int SaveChanges()
    {
        return SaveChanges(acceptAllChangesOnSuccess: true);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        if (_auditUserProvider == null || _suppressAuditLogging)
        {
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        return SaveChangesWithAuditing(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return SaveChangesAsync(acceptAllChangesOnSuccess: true, cancellationToken);
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        if (_auditUserProvider == null || _suppressAuditLogging)
        {
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        return await SaveChangesWithAuditingAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private int SaveChangesWithAuditing(bool acceptAllChangesOnSuccess)
    {
        var personID = _auditUserProvider!.GetCurrentPersonID();
        var changeDate = DateTime.Now;

        ChangeTracker.DetectChanges();

        var entries = ChangeTracker.Entries().ToList();
        var addedEntries = entries.Where(e => e.State == EntityState.Added).ToList();
        var modifiedOrDeletedEntries = entries.Where(e => e.State == EntityState.Modified || e.State == EntityState.Deleted).ToList();

        // Use an explicit transaction so both saves are atomic (matches legacy TransactionScope behavior)
        var existingTransaction = Database.CurrentTransaction;
        var transaction = existingTransaction == null ? Database.BeginTransaction() : null;
        try
        {
            // Phase 1: Create audit logs for Modified/Deleted (before save, while original values are available)
            foreach (var entry in modifiedOrDeletedEntries)
            {
                var auditRecords = AuditLogHelper.CreateAuditLogsForModifiedOrDeleted(entry, personID, changeDate);
                AuditLogs.AddRange(auditRecords);
            }

            // First save: persists data changes + Modified/Deleted audit logs
            var changes = base.SaveChanges(acceptAllChangesOnSuccess);

            // Phase 2: Create audit logs for Added (after save, so PKs are assigned)
            foreach (var entry in addedEntries)
            {
                var auditRecords = AuditLogHelper.CreateAuditLogsForAdded(entry, personID, changeDate);
                AuditLogs.AddRange(auditRecords);
            }

            // Second save: persists Added audit logs
            base.SaveChanges(acceptAllChangesOnSuccess);

            transaction?.Commit();
            return changes;
        }
        catch
        {
            transaction?.Rollback();
            throw;
        }
        finally
        {
            transaction?.Dispose();
        }
    }

    private async Task<int> SaveChangesWithAuditingAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
    {
        var personID = _auditUserProvider!.GetCurrentPersonID();
        var changeDate = DateTime.Now;

        ChangeTracker.DetectChanges();

        var entries = ChangeTracker.Entries().ToList();
        var addedEntries = entries.Where(e => e.State == EntityState.Added).ToList();
        var modifiedOrDeletedEntries = entries.Where(e => e.State == EntityState.Modified || e.State == EntityState.Deleted).ToList();

        // Use an explicit transaction so both saves are atomic (matches legacy TransactionScope behavior)
        var existingTransaction = Database.CurrentTransaction;
        var transaction = existingTransaction == null ? await Database.BeginTransactionAsync(cancellationToken) : null;
        try
        {
            // Phase 1: Create audit logs for Modified/Deleted (before save, while original values are available)
            foreach (var entry in modifiedOrDeletedEntries)
            {
                var auditRecords = AuditLogHelper.CreateAuditLogsForModifiedOrDeleted(entry, personID, changeDate);
                AuditLogs.AddRange(auditRecords);
            }

            // First save: persists data changes + Modified/Deleted audit logs
            var changes = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

            // Phase 2: Create audit logs for Added (after save, so PKs are assigned)
            foreach (var entry in addedEntries)
            {
                var auditRecords = AuditLogHelper.CreateAuditLogsForAdded(entry, personID, changeDate);
                AuditLogs.AddRange(auditRecords);
            }

            // Second save: persists Added audit logs
            await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

            if (transaction != null) await transaction.CommitAsync(cancellationToken);
            return changes;
        }
        catch
        {
            if (transaction != null) await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (transaction != null) await transaction.DisposeAsync();
        }
    }

    public async Task<int> SaveChangesWithNoAuditingAsync(CancellationToken cancellationToken = default)
    {
        _suppressAuditLogging = true;
        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        finally
        {
            _suppressAuditLogging = false;
        }
    }

    public int SaveChangesWithNoAuditing()
    {
        _suppressAuditLogging = true;
        try
        {
            return base.SaveChanges();
        }
        finally
        {
            _suppressAuditLogging = false;
        }
    }
}
