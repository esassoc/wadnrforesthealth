# 03 — Create Workflow: API Backend

> **Purpose**: Create workflow backend — progress computation, per-step Get/Save, state transitions, DTOs, controller endpoints.

---

## 1. Workflow Progress Class

**File**: `{EFModelsProject}/Workflows/{Entity}CreateWorkflowProgress.cs`

**Pattern from**: `WADNR.EFModels/Workflows/ProjectCreateWorkflowProgress.cs`

### Structure

```csharp
namespace {EFModelsProject}.Workflows;

public static class {Entity}CreateWorkflowProgress
{
    // 1. Step enum — all steps in order
    public enum {Entity}CreateWorkflowStep { ... }

    // 2. Private context class — holds all data needed for step computations
    private sealed class {Entity}CreateWorkflowContext { ... }

    // 3. Public entry points
    public static async Task<CreateWorkflowProgressResponse?> GetProgressAsync(...)
    public static async Task<CreateWorkflowProgressResponse?> GetProgressForUserAsync(...)
    public static async Task<bool> CanSubmitAsync(...)

    // 4. Private helpers
    private static async Task<{Entity}CreateWorkflowContext?> LoadWorkflowContextAsync(...)
    private static bool IsStepActive(...)
    private static bool IsStepComplete(...)
    private static bool IsStepRequired(...)
    private static bool CanSubmit(...)
    private static void PopulateUserPermissionFlags(...)
}
```

### Key Patterns

#### LoadWorkflowContext — Single Query

Load ALL data needed for step computations in ONE query using `.Select()`:

```csharp
private static async Task<{Entity}CreateWorkflowContext?> LoadWorkflowContextAsync(
    {DbContext} dbContext, int {entity}ID)
{
    return await dbContext.{Entities}
        .AsNoTracking()
        .Where(x => x.{Entity}ID == {entity}ID)
        .Select(x => new {Entity}CreateWorkflowContext
        {
            {Entity}ID = x.{Entity}ID,
            Name = x.Name,
            StatusID = x.StatusID,
            // Counts and flags for step completion:
            HasRelatedItems = x.RelatedItems.Any(),
            RelatedItemCount = x.RelatedItems.Count,
            // etc.
        })
        .SingleOrDefaultAsync();
}
```

#### IsStepComplete — Switch Expression

```csharp
private static bool IsStepComplete({Entity}CreateWorkflowContext ctx, {Entity}CreateWorkflowStep step)
{
    return step switch
    {
        {Entity}CreateWorkflowStep.Basics => IsBasicsComplete(ctx),
        {Entity}CreateWorkflowStep.StepTwo => ctx.HasStepTwoData,
        _ => false
    };
}
```

#### IsStepActive — First Step Always Active

```csharp
private static bool IsStepActive({Entity}CreateWorkflowContext ctx, {Entity}CreateWorkflowStep step)
{
    if (step == {Entity}CreateWorkflowStep.Basics) return true;
    return ctx.{Entity}ID > 0; // Other steps require entity to exist
}
```

#### IsStepRequired — Only Required Steps Gate Submission

```csharp
private static bool IsStepRequired({Entity}CreateWorkflowStep step)
{
    return step switch
    {
        {Entity}CreateWorkflowStep.Basics => true,
        {Entity}CreateWorkflowStep.AnotherRequired => true,
        _ => false
    };
}
```

#### User Permission Flags

```csharp
private static void PopulateUserPermissionFlags(
    CreateWorkflowProgressResponse dto,
    {Entity}CreateWorkflowContext ctx,
    PersonDetail? callingUser,
    {DbContext} dbContext)
{
    if (callingUser == null || callingUser.IsAnonymousOrUnassigned)
    {
        dto.CanApprove = false;
        dto.CanReject = false;
        dto.CanReturn = false;
        dto.CanWithdraw = false;
        dto.CanEdit = false;
        return;
    }

    var isPending = ctx.StatusID == (int){Entity}ApprovalStatusEnum.PendingApproval;
    var isDraftOrReturned = ctx.StatusID == (int){Entity}ApprovalStatusEnum.Draft ||
                            ctx.StatusID == (int){Entity}ApprovalStatusEnum.Returned;

    var canApprove = callingUser.HasElevatedProjectAccess || callingUser.HasCanEditProgramRole;
    dto.CanApprove = canApprove && isPending;
    dto.CanReject = canApprove && isPending;
    dto.CanReturn = canApprove && isPending;
    dto.CanWithdraw = isPending;
    dto.CanEdit = callingUser.HasElevatedProjectAccess || /* org-based check */;
}
```

---

## 2. Progress Response DTO

**File**: `{ModelsProject}/DataTransferObjects/{Entity}/Workflow/CreateWorkflowProgressResponse.cs`

Or place at bottom of progress class file (WADNR pattern).

```csharp
public class CreateWorkflowProgressResponse
{
    public int {Entity}ID { get; set; }
    public string {Entity}Name { get; set; } = string.Empty;
    public int {Entity}ApprovalStatusID { get; set; }
    public string {Entity}ApprovalStatusName { get; set; } = string.Empty;
    public bool CanSubmit { get; set; }
    public string? CreatedByPersonName { get; set; }
    public DateTime? CreateDate { get; set; }
    public Dictionary<{Entity}CreateWorkflowProgress.{Entity}CreateWorkflowStep, WorkflowStepStatus> Steps { get; set; } = new();

    // User permission flags
    public bool CanApprove { get; set; }
    public bool CanReject { get; set; }
    public bool CanReturn { get; set; }
    public bool CanWithdraw { get; set; }
    public bool CanEdit { get; set; }
}
```

---

## 3. Per-Step DTOs

**Directory**: `{ModelsProject}/DataTransferObjects/{Entity}/Workflow/`

### DTO Naming Convention

| DTO | Purpose |
|-----|---------|
| `{StepName}Step` | GET response — current step data |
| `{StepName}StepRequest` | PUT request — save step data |

### Step Type Templates

#### Form Step DTO

```csharp
public class {StepName}Step
{
    public int {Entity}ID { get; set; }
    public string? FieldA { get; set; }
    public int? FieldB { get; set; }
    // All fields the step displays
}

public class {StepName}StepRequest
{
    public string? FieldA { get; set; }
    public int? FieldB { get; set; }
    // All fields the step saves
}
```

#### Collection Step DTO

```csharp
public class {StepName}Step
{
    public int {Entity}ID { get; set; }
    public List<{StepName}Item> Items { get; set; } = new();
}

public class {StepName}Item
{
    public int? ItemID { get; set; } // null for new items
    public int RelatedEntityID { get; set; }
    public string? DisplayName { get; set; }
    // Other display fields
}

public class {StepName}StepRequest
{
    public List<{StepName}ItemRequest> Items { get; set; } = new();
}

public class {StepName}ItemRequest
{
    public int? ItemID { get; set; }
    public int RelatedEntityID { get; set; }
    // Other saveable fields
}
```

#### Geographic Assignment Step DTO

```csharp
public class GeographicAssignmentStep
{
    public int {Entity}ID { get; set; }
    public List<int> SelectedIDs { get; set; } = new();
    public string? NoSelectionExplanation { get; set; }
    public List<GeographicLookupItem> AvailableOptions { get; set; } = new();
}

public class GeographicLookupItem
{
    public int ID { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}

public class GeographicOverrideRequest
{
    public List<int> SelectedIDs { get; set; } = new();
    public string? NoSelectionExplanation { get; set; }
}
```

---

## 4. Workflow Steps Class

**File**: `{EFModelsProject}/Entities/{Entity}CreateWorkflowSteps.cs`

**Pattern from**: `WADNR.EFModels/Entities/ProjectCreateWorkflowSteps.cs`

### Structure

```csharp
public static class {Entity}CreateWorkflowSteps
{
    #region {StepName} Step
    public static async Task<{StepName}Step?> Get{StepName}StepAsync({DbContext} dbContext, int {entity}ID)
    public static async Task<{StepName}Step> CreateFrom{StepName}StepAsync(...)  // First step only
    public static async Task<{StepName}Step?> Save{StepName}StepAsync(...)
    #endregion

    // ... repeat for each step

    #region State Transitions
    public static async Task<WorkflowStateTransitionResponse> SubmitForApprovalAsync(...)
    public static async Task<WorkflowStateTransitionResponse> ApproveAsync(...)
    public static async Task<WorkflowStateTransitionResponse> ReturnAsync(...)
    public static async Task<WorkflowStateTransitionResponse> RejectAsync(...)
    public static async Task<WorkflowStateTransitionResponse> WithdrawAsync(...)
    #endregion
}
```

### Get Step Pattern

```csharp
public static async Task<{StepName}Step?> Get{StepName}StepAsync({DbContext} dbContext, int {entity}ID)
{
    return await dbContext.{Entities}
        .AsNoTracking()
        .Where(x => x.{Entity}ID == {entity}ID)
        .Select(x => new {StepName}Step
        {
            {Entity}ID = x.{Entity}ID,
            FieldA = x.FieldA,
            // Project only needed fields
        })
        .SingleOrDefaultAsync();
}
```

### First Step: Create + Save Variants

The first step has TWO variants:
1. **Create** (`CreateFrom{StepName}StepAsync`): Creates the entity, returns the step DTO
2. **Save** (`Save{StepName}StepAsync`): Updates an existing entity

```csharp
public static async Task<{StepName}Step> CreateFrom{StepName}StepAsync(
    {DbContext} dbContext, {StepName}StepRequest request, int callingPersonID)
{
    var entity = new {Entity}
    {
        // Map from request
        StatusID = (int){Entity}ApprovalStatusEnum.Draft,
        CreateDate = DateTime.UtcNow,
        CreatePersonID = callingPersonID
    };
    dbContext.{Entities}.Add(entity);
    await dbContext.SaveChangesAsync();
    return (await Get{StepName}StepAsync(dbContext, entity.{Entity}ID))!;
}
```

### Collection Sync Pattern

For steps with collections, use the sync pattern:

```csharp
public static async Task<{StepName}Step?> Save{StepName}StepAsync(
    {DbContext} dbContext, int {entity}ID, {StepName}StepRequest request)
{
    var entity = await dbContext.{Entities}
        .Include(x => x.{RelatedEntities})
        .FirstOrDefaultAsync(x => x.{Entity}ID == {entity}ID);
    if (entity == null) return null;

    // Sync: remove, update, add
    var existingIDs = entity.{RelatedEntities}.Select(x => x.ID).ToHashSet();
    var requestIDs = request.Items.Where(i => i.ID.HasValue).Select(i => i.ID!.Value).ToHashSet();

    // Remove not in request
    var toRemove = entity.{RelatedEntities}.Where(x => !requestIDs.Contains(x.ID)).ToList();
    dbContext.{RelatedEntities}.RemoveRange(toRemove);

    // Update existing and add new
    foreach (var item in request.Items) { ... }

    await dbContext.SaveChangesAsync();
    return await Get{StepName}StepAsync(dbContext, {entity}ID);
}
```

### Auto-Geographic-Assignment Pattern

When a location step is saved, automatically populate geographic join tables using spatial intersection queries. This runs after `SaveLocationSimpleStep` and `SaveLocationDetailedStep`:

```csharp
private static async Task AutoAssignGeographicRegionsAsync({DbContext} dbContext, int {entity}ID)
{
    // 1. Build a union geometry from all entity locations
    var locations = await dbContext.{Entity}Locations
        .Where(l => l.{Entity}ID == {entity}ID && l.{Entity}LocationGeometry != null)
        .Select(l => l.{Entity}LocationGeometry)
        .ToListAsync();

    Geometry? unionGeometry = null;
    if (locations.Any())
    {
        unionGeometry = locations[0];
        for (int i = 1; i < locations.Count; i++)
        {
            unionGeometry = unionGeometry.Union(locations[i]);
        }
    }

    // 2. For each geographic lookup table, find intersecting regions
    // Counties
    var existingCounties = await dbContext.{Entity}Counties
        .Where(x => x.{Entity}ID == {entity}ID).ToListAsync();
    dbContext.{Entity}Counties.RemoveRange(existingCounties);

    if (unionGeometry != null)
    {
        var matchingCounties = await dbContext.Counties
            .Where(c => c.CountyGeometry != null && c.CountyGeometry.Intersects(unionGeometry))
            .Select(c => c.CountyID)
            .ToListAsync();

        foreach (var countyID in matchingCounties)
        {
            dbContext.{Entity}Counties.Add(new {Entity}County
                { {Entity}ID = {entity}ID, CountyID = countyID });
        }
    }

    // Repeat for regions, priority landscapes, etc.
    await dbContext.SaveChangesAsync();
}
```

**When to call**: After `SaveLocationSimpleStep` and `SaveLocationDetailedStep`:
```csharp
public static async Task Save{LocationStep}Async(...)
{
    // ... save location data ...
    await dbContext.SaveChangesAsync();

    // Auto-assign geographic regions from new geometry
    await AutoAssignGeographicRegionsAsync(dbContext, {entity}ID);

    return await Get{LocationStep}Async(dbContext, {entity}ID);
}
```

### Unique Identifier Generation Pattern

If the entity needs a unique human-readable number (e.g., "FHT-0001"):

```csharp
private static async Task<string> GenerateEntityNumberAsync({DbContext} dbContext, string prefix = "FHT")
{
    var maxNumber = await dbContext.{Entities}
        .Where(x => x.EntityNumber != null && x.EntityNumber.StartsWith(prefix + "-"))
        .Select(x => x.EntityNumber)
        .MaxAsync();

    int nextNum = 1;
    if (maxNumber != null)
    {
        var parts = maxNumber.Split('-');
        if (parts.Length == 2 && int.TryParse(parts[1], out int parsed))
        {
            nextNum = parsed + 1;
        }
    }

    return $"{prefix}-{nextNum:D4}";
}
```

Used in `CreateFrom{StepName}StepAsync` when creating the entity.

---

### State Transitions

```csharp
public static async Task<WorkflowStateTransitionResponse> SubmitForApprovalAsync(
    {DbContext} dbContext, int {entity}ID, int callingPersonID, string? comment)
{
    var entity = await dbContext.{Entities}.FirstOrDefaultAsync(x => x.{Entity}ID == {entity}ID);
    if (entity == null) return new WorkflowStateTransitionResponse { Success = false, ErrorMessage = "Not found." };

    // Validate current status
    if (entity.StatusID != (int){Entity}ApprovalStatusEnum.Draft &&
        entity.StatusID != (int){Entity}ApprovalStatusEnum.Returned)
    {
        return new WorkflowStateTransitionResponse { Success = false, ErrorMessage = "Invalid status for submission." };
    }

    // Validate all required steps complete
    var canSubmit = await {Entity}CreateWorkflowProgress.CanSubmitAsync(dbContext, {entity}ID);
    if (!canSubmit) return new WorkflowStateTransitionResponse { Success = false, ErrorMessage = "Not all required steps complete." };

    entity.StatusID = (int){Entity}ApprovalStatusEnum.PendingApproval;
    entity.SubmissionDate = DateTime.UtcNow;
    await dbContext.SaveChangesAsync();

    return new WorkflowStateTransitionResponse { {Entity}ID = {entity}ID, Success = true, ... };
}
```

---

## 5. State Transition DTO

**File**: `{ModelsProject}/DataTransferObjects/{Entity}/Workflow/WorkflowStateTransition.cs`

```csharp
public class WorkflowStateTransitionResponse
{
    public int {Entity}ID { get; set; }
    public int NewStatusID { get; set; }
    public string? NewStatusName { get; set; }
    public DateTime TransitionDate { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

public class WorkflowStateTransitionRequest
{
    public string? Comment { get; set; }
}
```

---

## 6. Controller Endpoints

**File**: `{ApiProject}/Controllers/{Entity}Controller.cs`

Add a `#region Create Workflow` section.

### Endpoint Summary

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `{id}/create-workflow/progress` | `[ProjectEditFeature]` | Get step states |
| GET | `{id}/create-workflow/steps/{step}` | `[ProjectEditFeature]` | Get step data |
| PUT | `{id}/create-workflow/steps/{step}` | `[ProjectEditFeature]` | Save step data |
| POST | `create-workflow/create` | `[ProjectEditFeature]` | Create from first step |
| POST | `{id}/create-workflow/submit` | `[ProjectEditFeature]` | Submit for approval |
| POST | `{id}/create-workflow/approve` | `[ProjectApproveFeature]` | Approve |
| POST | `{id}/create-workflow/return` | `[ProjectApproveFeature]` | Return for revisions |
| POST | `{id}/create-workflow/reject` | `[ProjectApproveFeature]` | Reject |
| POST | `{id}/create-workflow/withdraw` | `[ProjectEditFeature]` | Withdraw submission |

### Progress Endpoint

```csharp
[HttpGet("{projectID}/create-workflow/progress")]
[ProjectEditFeature]
public async Task<ActionResult<CreateWorkflowProgressResponse>> GetCreateWorkflowProgress(
    [FromRoute] int projectID)
{
    var progress = await {Entity}CreateWorkflowProgress.GetProgressForUserAsync(DbContext, projectID, CallingUser);
    if (progress == null) return NotFound();
    return Ok(progress);
}
```

### Per-Step GET/PUT

```csharp
[HttpGet("{projectID}/create-workflow/steps/basics")]
[ProjectEditFeature]
public async Task<ActionResult<ProjectBasicsStep>> GetCreateBasicsStep([FromRoute] int projectID)
{
    var step = await {Entity}CreateWorkflowSteps.GetBasicsStepAsync(DbContext, projectID);
    if (step == null) return NotFound();
    return Ok(step);
}

[HttpPut("{projectID}/create-workflow/steps/basics")]
[ProjectEditFeature]
public async Task<ActionResult<ProjectBasicsStep>> SaveCreateBasicsStep(
    [FromRoute] int projectID, [FromBody] ProjectBasicsStepRequest request)
{
    var step = await {Entity}CreateWorkflowSteps.SaveBasicsStepAsync(DbContext, projectID, request, CallingUser.PersonID);
    if (step == null) return NotFound();
    return Ok(step);
}
```

### Create (First Step POST)

```csharp
[HttpPost("create-workflow/create")]
[ProjectEditFeature]
public async Task<ActionResult<ProjectBasicsStep>> CreateFromBasicsStep(
    [FromBody] ProjectBasicsStepRequest request)
{
    var step = await {Entity}CreateWorkflowSteps.CreateProjectFromBasicsStepAsync(
        DbContext, request, CallingUser.PersonID);
    return Ok(step);
}
```

### State Transition Endpoints

```csharp
[HttpPost("{projectID}/create-workflow/submit")]
[ProjectEditFeature]
public async Task<ActionResult<WorkflowStateTransitionResponse>> SubmitCreateForApproval(
    [FromRoute] int projectID, [FromBody] WorkflowStateTransitionRequest request)
{
    var result = await {Entity}CreateWorkflowSteps.SubmitForApprovalAsync(
        DbContext, projectID, CallingUser.PersonID, request?.Comment);
    if (!result.Success) return BadRequest(result);
    return Ok(result);
}
```

---

## 7. After This Phase

Build the API and regenerate TypeScript:

```powershell
dotnet build WADNR.sln
cd WADNR.Web
npm run gen-model
```

Verify generated files include:
- `create-workflow-progress-response.ts`
- Per-step DTOs
- Service methods for all endpoints
