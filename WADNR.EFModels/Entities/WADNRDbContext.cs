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

        // If a caller already owns a transaction, run the work inline — the caller is responsible
        // for atomicity and retry semantics (they wrap their work in CreateExecutionStrategy themselves).
        if (Database.CurrentTransaction != null)
        {
            return DoAuditingSave(personID, changeDate, acceptAllChangesOnSuccess);
        }

        // No outer transaction — wrap our two-phase save in an execution strategy. EnableRetryOnFailure
        // rejects user-initiated transactions opened outside a strategy, so the transaction must live
        // inside strategy.Execute. Snapshot pre-existing audit logs so retries can distinguish
        // caller-added entries (preserve) from ones our Phase 1/Phase 2 added (detach to avoid dupes).
        var preExistingAuditLogs = new HashSet<AuditLog>(ChangeTracker.Entries<AuditLog>().Select(e => e.Entity));

        var strategy = Database.CreateExecutionStrategy();
        return strategy.Execute(() =>
        {
            DetachAuditLogsAddedByPriorAttempt(preExistingAuditLogs);

            using var transaction = Database.BeginTransaction();
            var changes = DoAuditingSave(personID, changeDate, acceptAllChangesOnSuccess);
            transaction.Commit();
            return changes;
        });
    }

    private async Task<int> SaveChangesWithAuditingAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
    {
        var personID = _auditUserProvider!.GetCurrentPersonID();
        var changeDate = DateTime.Now;

        if (Database.CurrentTransaction != null)
        {
            return await DoAuditingSaveAsync(personID, changeDate, acceptAllChangesOnSuccess, cancellationToken);
        }

        var preExistingAuditLogs = new HashSet<AuditLog>(ChangeTracker.Entries<AuditLog>().Select(e => e.Entity));

        var strategy = Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            DetachAuditLogsAddedByPriorAttempt(preExistingAuditLogs);

            await using var transaction = await Database.BeginTransactionAsync(cancellationToken);
            var changes = await DoAuditingSaveAsync(personID, changeDate, acceptAllChangesOnSuccess, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return changes;
        });
    }

    // Detach AuditLog entries this auditing flow added in a previous (failed) strategy attempt.
    // Caller-added audit logs (in `preExistingAuditLogs`) are preserved.
    private void DetachAuditLogsAddedByPriorAttempt(HashSet<AuditLog> preExistingAuditLogs)
    {
        var stale = ChangeTracker.Entries<AuditLog>()
            .Where(e => e.State == EntityState.Added && !preExistingAuditLogs.Contains(e.Entity))
            .ToList();
        foreach (var entry in stale)
        {
            entry.State = EntityState.Detached;
        }
    }

    private int DoAuditingSave(int personID, DateTime changeDate, bool acceptAllChangesOnSuccess)
    {
        ChangeTracker.DetectChanges();

        var entries = ChangeTracker.Entries().ToList();
        var addedEntries = entries.Where(e => e.State == EntityState.Added && e.Entity is not AuditLog).ToList();
        var modifiedOrDeletedEntries = entries.Where(e => e.State == EntityState.Modified || e.State == EntityState.Deleted).ToList();

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

        return changes;
    }

    private async Task<int> DoAuditingSaveAsync(int personID, DateTime changeDate, bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
    {
        ChangeTracker.DetectChanges();

        var entries = ChangeTracker.Entries().ToList();
        var addedEntries = entries.Where(e => e.State == EntityState.Added && e.Entity is not AuditLog).ToList();
        var modifiedOrDeletedEntries = entries.Where(e => e.State == EntityState.Modified || e.State == EntityState.Deleted).ToList();

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

        return changes;
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
