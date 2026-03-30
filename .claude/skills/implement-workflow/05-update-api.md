# 05 — Update Workflow: API Backend

> **Purpose**: Update workflow backend — batch management, per-step Get/Save/Revert, diff generation, state transitions, controller endpoints.

---

## 1. Update Workflow Progress Class

**File**: `{EFModelsProject}/Workflows/{Entity}UpdateWorkflowProgress.cs`

**Pattern from**: `WADNR.EFModels/Workflows/ProjectUpdateWorkflowProgress.cs`

### Key Differences from Create Progress

| Aspect | Create | Update |
|--------|--------|--------|
| Primary key | `{Entity}ID` | `{Entity}UpdateBatchID` |
| Step dict key type | Enum | `string` (PascalCase step name) |
| Additional per-step state | — | `HasChanges` |
| Reviewer comments | — | `ReviewerComments` dict |
| Ready to approve | — | `IsReadyToApprove` |
| Context data source | Entity tables | Update mirror tables |

### Structure

```csharp
public static class {Entity}UpdateWorkflowProgress
{
    public enum {Entity}UpdateWorkflowStep { Basics = 1, StepTwo = 2, ... }

    // Use sealed record (not class) for the with {} syntax
    private sealed record {Entity}UpdateWorkflowContext { ... }

    public static async Task<UpdateWorkflowProgressResponse?> GetProgressAsync(...)
    public static async Task<UpdateWorkflowProgressResponse?> GetProgressForUserAsync(...)
    public static async Task<bool> CanSubmitAsync(...)

    private static async Task<{Entity}UpdateWorkflowContext?> LoadWorkflowContextAsync(...)
    private static async Task<Dictionary<string, bool>> GetStepChangesAsync(...)  // New!
    private static Dictionary<string, string?> BuildReviewerComments(...)  // New!
    private static bool IsStepActive(...)  // Usually all true for Update
    private static bool IsStepComplete(...)
    private static bool IsStepRequired(...)
    private static bool CanSubmit(...)
    private static bool IsReadyToApprove(...)  // New!
    private static void PopulateUserPermissionFlags(...)
}
```

### LoadWorkflowContext

Queries from `{Entity}UpdateBatch` and its related Update tables:

```csharp
private static async Task<{Entity}UpdateWorkflowContext?> LoadWorkflowContextAsync(
    {DbContext} dbContext, int batchID)
{
    var batch = await dbContext.{Entity}UpdateBatches
        .AsNoTracking()
        .Where(b => b.{Entity}UpdateBatchID == batchID)
        .Select(b => new {Entity}UpdateWorkflowContext
        {
            {Entity}UpdateBatchID = b.{Entity}UpdateBatchID,
            {Entity}ID = b.{Entity}ID,
            {Entity}Name = b.{Entity}.Name,
            StateID = b.{Entity}UpdateStateID,
            StateName = null, // Resolved client-side below
            HasRelatedUpdates = b.{RelatedEntity}Updates.Any(),
            // Reviewer comment columns
            BasicsComment = b.BasicsComment,
            // ... other comment columns
        })
        .SingleOrDefaultAsync();

    if (batch == null) return null;

    // Resolve lookup value client-side (Rule 5 from dotnet-patterns)
    if ({Entity}UpdateState.AllLookupDictionary.TryGetValue(batch.StateID, out var state))
    {
        batch = batch with { StateName = state.DisplayName };
    }

    // Fetch submitted/returned history
    var histories = await dbContext.{Entity}UpdateHistories
        .AsNoTracking()
        .Where(h => h.{Entity}UpdateBatchID == batchID)
        .OrderByDescending(h => h.TransitionDate)
        .Select(h => new { h.{Entity}UpdateStateID, h.TransitionDate,
            PersonName = h.UpdatePerson.FirstName + " " + h.UpdatePerson.LastName })
        .ToListAsync();

    // Populate submitted/returned metadata from history
    // ...

    return batch;
}
```

### GetStepChangesAsync — HasChanges Per Step

Compares Update data to live entity data. Load each collection pair separately to avoid Cartesian product joins:

```csharp
private static async Task<Dictionary<string, bool>> GetStepChangesAsync(
    {DbContext} dbContext, int batchID, int {entity}ID)
{
    var result = new Dictionary<string, bool>();

    // Load scalar data
    var entity = await dbContext.{Entities}.AsNoTracking().FirstOrDefaultAsync(...);
    var update = await dbContext.{Entity}Updates.AsNoTracking().FirstOrDefaultAsync(...);

    // Compare basics
    result["Basics"] = HasBasicsChanges(entity, update);

    // Compare each collection separately
    var liveItems = await dbContext.RelatedItems.AsNoTracking()
        .Where(x => x.{Entity}ID == {entity}ID).ToListAsync();
    var updateItems = await dbContext.RelatedItemUpdates.AsNoTracking()
        .Where(x => x.{Entity}UpdateBatchID == batchID).ToListAsync();
    result["StepKey"] = HasCollectionChanges(liveItems, updateItems);

    // ... repeat for each step
    return result;
}
```

### Comparison Helpers

```csharp
// Simple field comparison
private static bool HasBasicsChanges({Entity} entity, {Entity}Update? update)
{
    if (update == null) return false;
    if (entity.FieldA != update.FieldA) return true;
    if (entity.FieldB != update.FieldB) return true;
    return false;
}

// Collection comparison by key
private static bool HasCollectionChanges<TLive, TUpdate>(
    List<TLive> live, List<TUpdate> update,
    Func<TLive, int> liveKeySelector, Func<TUpdate, int> updateKeySelector)
{
    if (live.Count != update.Count) return true;
    var liveKeys = live.Select(liveKeySelector).OrderBy(x => x).ToList();
    var updateKeys = update.Select(updateKeySelector).OrderBy(x => x).ToList();
    return !liveKeys.SequenceEqual(updateKeys);
}

// Geographic comparison (IDs + explanation)
private static bool HasGeographicChanges(
    HashSet<int> liveIDs, HashSet<int> updateIDs,
    string? liveExplanation, string? updateExplanation)
{
    if (!liveIDs.SetEquals(updateIDs)) return true;
    return liveExplanation != updateExplanation;
}
```

### BuildReviewerComments

Returns a dictionary mapping PascalCase step keys to reviewer comment text. Only populated when batch is in Returned state.

```csharp
private static Dictionary<string, string?> BuildReviewerComments({Entity}UpdateWorkflowContext ctx)
{
    return new Dictionary<string, string?>
    {
        ["Basics"] = ctx.BasicsComment,
        ["Organizations"] = ctx.OrganizationsComment,
        // Geographic steps may share the same comment
        ["PriorityLandscapes"] = ctx.LocationComment,
        // ... etc.
    };
}
```

### Response DTO

```csharp
public class UpdateWorkflowProgressResponse
{
    public int {Entity}UpdateBatchID { get; set; }
    public int {Entity}ID { get; set; }
    public string {Entity}Name { get; set; } = string.Empty;
    public int {Entity}UpdateStateID { get; set; }
    public string? {Entity}UpdateStateName { get; set; }
    public DateTime LastUpdateDate { get; set; }
    public string? LastUpdatedByPersonName { get; set; }
    public string? SubmittedByPersonName { get; set; }
    public DateTime? SubmittedDate { get; set; }
    public string? ReturnedByPersonName { get; set; }
    public DateTime? ReturnedDate { get; set; }
    public bool CanSubmit { get; set; }
    public bool IsReadyToApprove { get; set; }
    public Dictionary<string, WorkflowStepStatus> Steps { get; set; } = new();
    public Dictionary<string, string?>? ReviewerComments { get; set; }

    // User permission flags
    public bool CanEdit { get; set; }
    public bool CanApprove { get; set; }
    public bool CanReturn { get; set; }
    public bool CanDelete { get; set; }
}
```

---

## 2. Update Workflow Steps Class

**File**: `{EFModelsProject}/Entities/{Entity}UpdateWorkflowSteps.cs`

**Pattern from**: `WADNR.EFModels/Entities/ProjectUpdateWorkflowSteps.cs`

### Structure

```csharp
public static class {Entity}UpdateWorkflowSteps
{
    #region Batch Management
    public static async Task<BatchDetail?> StartBatchAsync(...)    // Calls stored proc
    public static async Task<BatchDetail?> GetCurrentBatchAsync(...)
    public static async Task<bool> DeleteBatchAsync(...)           // Cascade cleanup
    #endregion

    #region {StepName} Step
    public static async Task<{StepName}UpdateStep?> Get{StepName}StepAsync(...)
    public static async Task<{StepName}UpdateStep?> Save{StepName}StepAsync(...)
    public static async Task Revert{StepName}StepAsync(...)        // New!
    #endregion

    // ... repeat for each step

    #region State Transitions
    public static async Task SubmitForApprovalAsync(...)  // Generates diffs
    public static async Task ApproveAsync(...)            // Copies update→live
    public static async Task ReturnAsync(...)             // Stores per-step comments
    #endregion
}
```

### StartBatchAsync

```csharp
public static async Task<BatchDetail?> StartBatchAsync(
    {DbContext} dbContext, int {entity}ID, int callingPersonID)
{
    var entity = await dbContext.{Entities}.FirstOrDefaultAsync(x => x.{Entity}ID == {entity}ID);
    if (entity == null) return null;

    if (entity.StatusID != (int){Entity}ApprovalStatusEnum.Approved)
        throw new InvalidOperationException("Only Approved entities can start updates.");

    var latestBatch = await dbContext.{Entity}UpdateBatches
        .Where(b => b.{Entity}ID == {entity}ID)
        .OrderByDescending(b => b.{Entity}UpdateBatchID)
        .FirstOrDefaultAsync();
    if (latestBatch != null && latestBatch.StateID != (int){Entity}UpdateStateEnum.Approved)
        throw new InvalidOperationException("An update batch is already in progress.");

    await dbContext.Database.ExecuteSqlInterpolatedAsync(
        $"EXEC dbo.pStart{Entity}UpdateBatch @{Entity}ID={{entity}ID}, @CallingPersonID={callingPersonID}");

    return await GetCurrentBatchAsync(dbContext, {entity}ID);
}
```

### DeleteBatchAsync — Cascade Cleanup

Load each child collection separately (not via Include) to avoid Cartesian product joins:

```csharp
public static async Task<bool> DeleteBatchAsync({DbContext} dbContext, int batchID, int callingPersonID)
{
    var batch = await dbContext.{Entity}UpdateBatches.FirstOrDefaultAsync(b => b.ID == batchID);
    if (batch == null) return false;

    // Validate state
    if (batch.StateID != Created && batch.StateID != Returned)
        throw new InvalidOperationException("Can only delete in Created or Returned state.");

    // Load each collection separately
    var scalarUpdates = await dbContext.{Entity}Updates.Where(x => x.BatchID == batchID).ToListAsync();
    var relatedUpdates = await dbContext.{Related}Updates.Where(x => x.BatchID == batchID).ToListAsync();
    // ... load all collections

    // Handle orphaned file resources (photos/docs uploaded during update, not shared with live)
    var imageFiles = imageUpdates.Where(i => i.FileResourceID.HasValue).Select(i => i.FileResourceID!.Value).ToList();
    var sharedFiles = await dbContext.{Entity}Images.Where(i => imageFiles.Contains(i.FileResourceID))
        .Select(i => i.FileResourceID).ToListAsync();
    var orphanedFiles = imageUpdates.Where(i => !sharedFiles.Contains(i.FileResourceID)).Select(i => i.FileResource!);

    // Remove all in correct order (respect FK dependencies)
    dbContext.RemoveRange(treatmentUpdates); // Before locations
    dbContext.RemoveRange(locationUpdates);
    // ... remove all
    dbContext.RemoveRange(orphanedFiles);
    dbContext.{Entity}UpdateBatches.Remove(batch);
    await dbContext.SaveChangesAsync();
    return true;
}
```

### Per-Step Get/Save (Update Tables)

Same pattern as Create but queries Update mirror tables via batch:

```csharp
public static async Task<{StepName}UpdateStep?> Get{StepName}StepAsync(
    {DbContext} dbContext, int batchID)
{
    return await dbContext.{Entity}UpdateBatches
        .AsNoTracking()
        .Where(b => b.{Entity}UpdateBatchID == batchID)
        .Select(b => new {StepName}UpdateStep
        {
            BatchID = b.{Entity}UpdateBatchID,
            // Query from Update tables, not live tables
            Items = b.{Related}Updates.Select(x => new ...).ToList()
        })
        .SingleOrDefaultAsync();
}
```

### Per-Step Revert

Clear Update rows and re-copy from live:

```csharp
public static async Task Revert{StepName}StepAsync({DbContext} dbContext, int batchID)
{
    var batch = await dbContext.{Entity}UpdateBatches.FirstOrDefaultAsync(b => b.ID == batchID);
    if (batch == null) return;
    VerifyBatchIsEditable(batch);

    // Remove existing update rows for this step
    var existing = await dbContext.{Related}Updates
        .Where(x => x.BatchID == batchID).ToListAsync();
    dbContext.{Related}Updates.RemoveRange(existing);

    // Re-copy from live tables
    var live = await dbContext.{Related}
        .Where(x => x.{Entity}ID == batch.{Entity}ID).ToListAsync();
    foreach (var item in live)
    {
        dbContext.{Related}Updates.Add(new {Related}Update
        {
            {Entity}UpdateBatchID = batchID,
            // Copy fields from live item
        });
    }

    batch.LastUpdateDate = DateTime.UtcNow;
    await dbContext.SaveChangesAsync();
}
```

### General Revert Dispatcher

A single revert endpoint can dispatch by step key:

```csharp
public static async Task RevertStepAsync({DbContext} dbContext, int batchID, string stepKey)
{
    var normalizedKey = Regex.Replace(stepKey, "([a-z])([A-Z])", "$1-$2").ToLowerInvariant();
    switch (normalizedKey)
    {
        case "basics": await RevertBasicsStepAsync(dbContext, batchID); break;
        case "organizations": await RevertOrganizationsStepAsync(dbContext, batchID); break;
        // ... all steps
        default: throw new ArgumentException($"Unknown step key: {stepKey}");
    }
}
```

### VerifyBatchIsEditable Guard

```csharp
private static void VerifyBatchIsEditable({Entity}UpdateBatch batch)
{
    if (batch.StateID != (int){Entity}UpdateStateEnum.Created &&
        batch.StateID != (int){Entity}UpdateStateEnum.Returned)
    {
        throw new InvalidOperationException("Batch is not in an editable state.");
    }
}
```

### State Transitions

#### Submit — Generates Diffs

```csharp
public static async Task SubmitForApprovalAsync({DbContext} dbContext, int batchID, int callingPersonID)
{
    var batch = await dbContext.{Entity}UpdateBatches.FirstOrDefaultAsync(b => b.ID == batchID);
    VerifyBatchIsEditable(batch);

    // Generate and store diffs for audit trail
    await {Entity}UpdateDiffs.GenerateAndStoreDiffsAsync(dbContext, batch);

    batch.StateID = (int){Entity}UpdateStateEnum.Submitted;
    batch.LastUpdateDate = DateTime.UtcNow;
    batch.LastUpdatePersonID = callingPersonID;

    // Log history
    dbContext.{Entity}UpdateHistories.Add(new {Entity}UpdateHistory { ... });
    await dbContext.SaveChangesAsync();
}
```

#### Approve — Copy Update → Live

```csharp
public static async Task ApproveAsync({DbContext} dbContext, int batchID, int callingPersonID)
{
    var batch = await dbContext.{Entity}UpdateBatches.FirstOrDefaultAsync(b => b.ID == batchID);
    if (batch.StateID != (int){Entity}UpdateStateEnum.Submitted)
        throw new InvalidOperationException("Can only approve submitted batches.");

    // Copy scalars from {Entity}Update to {Entity}
    var entity = await dbContext.{Entities}.FirstOrDefaultAsync(x => x.ID == batch.{Entity}ID);
    var update = await dbContext.{Entity}Updates.FirstOrDefaultAsync(x => x.BatchID == batchID);
    entity.FieldA = update.FieldA;
    // ... copy all scalar fields

    // For each collection: clear live, copy from Update
    // ... sync pattern for each related table

    batch.StateID = (int){Entity}UpdateStateEnum.Approved;
    dbContext.{Entity}UpdateHistories.Add(new ... { StateID = Approved });
    await dbContext.SaveChangesAsync();
}
```

#### Return — Stores Per-Step Comments

```csharp
public static async Task ReturnAsync({DbContext} dbContext, int batchID, int callingPersonID,
    Dictionary<string, string?> stepComments)
{
    var batch = await dbContext.{Entity}UpdateBatches.FirstOrDefaultAsync(b => b.ID == batchID);
    if (batch.StateID != (int){Entity}UpdateStateEnum.Submitted)
        throw new InvalidOperationException("Can only return submitted batches.");

    // Store per-step comments on batch
    batch.BasicsComment = stepComments.GetValueOrDefault("Basics");
    batch.OrganizationsComment = stepComments.GetValueOrDefault("Organizations");
    // ... etc.

    batch.StateID = (int){Entity}UpdateStateEnum.Returned;
    dbContext.{Entity}UpdateHistories.Add(new ... { StateID = Returned });
    await dbContext.SaveChangesAsync();
}
```

---

## 3. Diff Generation Class

**File**: `{EFModelsProject}/Entities/{Entity}UpdateDiffs.cs`

**Pattern from**: `WADNR.EFModels/Entities/ProjectUpdateDiffs.cs`

### Structure

```csharp
public static class {Entity}UpdateDiffs
{
    // Entry point: generates all diffs and stores on batch
    public static async Task GenerateAndStoreDiffsAsync({DbContext} dbContext, {Entity}UpdateBatch batch)

    // Per-step diff: called by controller for individual step diff viewing
    public static async Task<StepDiffResponse> GetStepDiffAsync(
        {DbContext} dbContext, int batchID, string stepKey)

    // Per-step diff methods
    private static async Task<StepDiffResponse> Get{StepName}DiffAsync(...)
}
```

### StepDiffResponse DTO

```csharp
public class StepDiffResponse
{
    public bool HasChanges { get; set; }
    public List<DiffSection> Sections { get; set; } = new();
}

public class DiffSection
{
    public string? Title { get; set; }
    public string Type { get; set; } = "fields"; // "fields", "list", "table"
    public List<DiffField>? Fields { get; set; }
    public List<string>? OriginalItems { get; set; }
    public List<string>? UpdatedItems { get; set; }
    public List<string>? Headers { get; set; }
    public List<string[]>? OriginalRows { get; set; }
    public List<string[]>? UpdatedRows { get; set; }
}

public class DiffField
{
    public string Label { get; set; } = string.Empty;
    public string? OriginalValue { get; set; }
    public string? UpdatedValue { get; set; }
}
```

### Step Key Normalization

```csharp
public static async Task<StepDiffResponse> GetStepDiffAsync(
    {DbContext} dbContext, int batchID, string stepKey)
{
    var normalizedKey = Regex.Replace(stepKey, "([a-z])([A-Z])", "$1-$2").ToLowerInvariant();
    return normalizedKey switch
    {
        "basics" => await GetBasicsDiffAsync(dbContext, batchID),
        "organizations" => await GetOrganizationsDiffAsync(dbContext, batchID),
        // ... all steps
        _ => new StepDiffResponse { HasChanges = false }
    };
}
```

### Per-Step Diff Example: Fields Type

```csharp
private static async Task<StepDiffResponse> GetBasicsDiffAsync({DbContext} dbContext, int batchID)
{
    var batch = await dbContext.{Entity}UpdateBatches.AsNoTracking()
        .FirstOrDefaultAsync(b => b.ID == batchID);
    var entity = await dbContext.{Entities}.AsNoTracking()
        .FirstOrDefaultAsync(x => x.ID == batch.{Entity}ID);
    var update = await dbContext.{Entity}Updates.AsNoTracking()
        .FirstOrDefaultAsync(x => x.BatchID == batchID);

    var fields = new List<DiffField>
    {
        new() { Label = "Field A", OriginalValue = entity.FieldA, UpdatedValue = update.FieldA },
        new() { Label = "Field B", OriginalValue = entity.FieldB?.ToString(), UpdatedValue = update.FieldB?.ToString() },
    };

    var hasChanges = fields.Any(f => (f.OriginalValue ?? "") != (f.UpdatedValue ?? ""));

    return new StepDiffResponse
    {
        HasChanges = hasChanges,
        Sections = new() { new DiffSection { Type = "fields", Fields = fields } }
    };
}
```

### Per-Step Diff Example: List Type

```csharp
private static async Task<StepDiffResponse> GetGeographicDiffAsync(...)
{
    var liveIDs = await dbContext.{Related}.Where(x => x.{Entity}ID == entityID)
        .Select(x => x.LookupName).ToListAsync();
    var updateIDs = await dbContext.{Related}Updates.Where(x => x.BatchID == batchID)
        .Select(x => x.LookupName).ToListAsync();

    return new StepDiffResponse
    {
        HasChanges = !liveIDs.OrderBy(x => x).SequenceEqual(updateIDs.OrderBy(x => x)),
        Sections = new() { new DiffSection
        {
            Type = "list",
            OriginalItems = liveIDs,
            UpdatedItems = updateIDs
        }}
    };
}
```

### Per-Step Diff Example: Table Type

```csharp
private static async Task<StepDiffResponse> GetOrganizationsDiffAsync(...)
{
    var liveOrgs = await dbContext.{Entity}Organizations.AsNoTracking()
        .Where(x => x.{Entity}ID == entityID)
        .Select(x => new string[] { x.Organization.Name, x.RelationshipType.Name })
        .ToListAsync();
    var updateOrgs = await dbContext.{Entity}OrganizationUpdates.AsNoTracking()
        .Where(x => x.BatchID == batchID)
        .Select(x => new string[] { x.Organization.Name, x.RelationshipType.Name })
        .ToListAsync();

    return new StepDiffResponse
    {
        HasChanges = liveOrgs.Count != updateOrgs.Count || /* row comparison */,
        Sections = new() { new DiffSection
        {
            Type = "table",
            Headers = new() { "Organization", "Relationship" },
            OriginalRows = liveOrgs,
            UpdatedRows = updateOrgs
        }}
    };
}
```

---

## 4. Controller Endpoints

Add a `#region Update Workflow` section to the controller.

### Endpoint Summary

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `{id}/update-workflow/start` | `[ProjectEditFeature]` | Start new batch |
| DELETE | `{id}/update-workflow` | `[ProjectEditFeature]` | Delete batch |
| GET | `{id}/update-workflow/progress` | `[ProjectEditFeature]` | Get progress + has-changes |
| GET | `{id}/update-workflow/steps/{step}` | `[ProjectEditFeature]` | Get step data |
| PUT | `{id}/update-workflow/steps/{step}` | `[ProjectEditFeature]` | Save step data |
| POST | `{id}/update-workflow/steps/{step}/revert` | `[ProjectEditFeature]` | Revert step |
| GET | `{id}/update-workflow/steps/{step}/diff` | `[ProjectEditFeature]` | Get step diff |
| POST | `{id}/update-workflow/submit` | `[ProjectEditFeature]` | Submit for approval |
| POST | `{id}/update-workflow/approve` | `[ProjectApproveFeature]` | Approve (copy update→live) |
| POST | `{id}/update-workflow/return` | `[ProjectApproveFeature]` | Return with per-step comments |
| GET | `{id}/update-workflow/history` | `[ProjectEditFeature]` | List batch history entries |

### Key Endpoint: Return with Comments

```csharp
[HttpPost("{projectID}/update-workflow/return")]
[ProjectApproveFeature]
public async Task<IActionResult> ReturnUpdate(
    [FromRoute] int projectID, [FromBody] Dictionary<string, string?> stepComments)
{
    var batch = await {Entity}UpdateWorkflowSteps.GetCurrentBatchAsync(DbContext, projectID);
    if (batch == null) return NotFound();
    await {Entity}UpdateWorkflowSteps.ReturnAsync(DbContext, batch.ID, CallingUser.PersonID, stepComments);
    return Ok();
}
```

---

## 5. After This Phase

Build the API and regenerate TypeScript:

```powershell
dotnet build WADNR.sln
cd WADNR.Web
npm run gen-model
```

Verify generated files include:
- `update-workflow-progress-response.ts`
- `step-diff-response.ts`, `diff-section.ts`, `diff-field.ts`
- Per-step Update DTOs
- Service methods for all update workflow endpoints
