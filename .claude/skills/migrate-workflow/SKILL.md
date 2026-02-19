# Migrate Workflow Skill

> **Scope**: fullstack
> **Prereqs**: Load `/dotnet-patterns` and `/angular-patterns` first

When the user invokes `/migrate-workflow <EntityName>`:

This skill guides migration of multi-step wizard workflows from legacy MVC to Angular + ASP.NET Core API. Use it for entities with sequential data entry steps, such as registrations, assessments, or multi-page forms.

## Contents

1. [Analyze Legacy Workflow](#1-analyze-legacy-workflow)
2. [Backend: Workflow Progress Class](#2-backend-workflow-progress-class)
3. [Backend: Step DTOs](#3-backend-step-dtos)
4. [Backend: Controller Workflow Endpoints](#4-backend-controller-workflow-endpoints)
5. [Backend: State Transitions](#5-backend-state-transitions)
6. [Frontend: State Transition Actions](#6-frontend-state-transition-actions)
7. [Generate TypeScript](#7-generate-typescript)
8. [Frontend: Workflow Progress Service](#8-frontend-workflow-progress-service)
9. [Frontend: Workflow Outlet Component](#9-frontend-workflow-outlet-component)
10. [Frontend: Step Components](#10-frontend-step-components)
11. [Frontend: Route Configuration](#11-frontend-route-configuration)
12. [Checklist Before Completion](#12-checklist-before-completion)

## Prerequisites

- Basic entity CRUD already exists (or will be created as part of step 1)
- Reference implementations: Road Registration (6 steps), BMP Registration (7 steps)

---

## 1. Analyze Legacy Workflow

First, thoroughly examine the existing MVC implementation:

- Read the legacy controller: `{LegacyPath}/Controllers/{Entity}Controller.cs`
- Read workflow views in: `{LegacyPath}/Views/{Entity}/` (look for step-specific views)
- Identify workflow action methods (e.g., `Step1`, `Step2`, `Submit`)

Document for each step:
- Step name and purpose
- Fields/data collected
- Validation rules
- Dependencies (which steps must complete first)
- Step type: form entry, map selection, file upload, grid/list management, or review

### Step Type Reference

| Step Type | Pattern | Examples |
|-----------|---------|----------|
| Form entry | Reactive form, save to entity | Registration Information, Contact Details |
| Map selection | Map component + geometry binding | Registration Area, Site Location |
| File upload | `[FromForm]` + file processing | PLRM Upload, Document Attachments |
| Grid/list management | Add/remove items from collection | Register BMPs, Select Parcels |
| Review/submit | Read-only summary + submit action | Review and Finalize |

---

## 2. Backend: Workflow Progress Class

Create `{EFModelsProject}/Workflows/{Entity}WorkflowProgress.cs`:

```csharp
using {EFModelsProject}.Entities;
using {ModelsProject}.DataTransferObjects;
using Microsoft.EntityFrameworkCore;

namespace {EFModelsProject}.Workflows;

public static class {Entity}WorkflowProgress
{
    public enum {Entity}WorkflowStep
    {
        Step1Name,
        Step2Name,
        Step3Name,
        // ... all steps
    }

    // Public entry point: loads context and computes step states
    public static async Task<{Entity}WorkflowProgressDto?> GetProgressAsync(
        {DbContext} dbContext,
        int {entity}ID)
    {
        var ctx = await LoadWorkflowContextAsync(dbContext, {entity}ID);
        if (ctx == null) return null;

        return new {Entity}WorkflowProgressDto
        {
            {Entity}ID = ctx.{Entity}ID,
            Name = ctx.Name,
            // ... other header fields
            Steps = Enum.GetValuesAsUnderlyingType<{Entity}WorkflowStep>()
                .Cast<{Entity}WorkflowStep>()
                .ToDictionary(
                    step => step,
                    step => new WorkflowStepStatus
                    {
                        Completed = IsStepComplete(ctx, step),
                        Disabled = !IsStepActive(ctx, step)
                    })
        };
    }

    public static async Task<bool> CanSubmitAsync({DbContext} dbContext, int {entity}ID)
    {
        var ctx = await LoadWorkflowContextAsync(dbContext, {entity}ID);
        if (ctx == null) return false;

        return Enum.GetValuesAsUnderlyingType<{Entity}WorkflowStep>()
            .Cast<{Entity}WorkflowStep>()
            .All(step => IsStepComplete(ctx, step));
    }

    private static bool IsStepActive({Entity}WorkflowContext ctx, {Entity}WorkflowStep step)
    {
        return step switch
        {
            {Entity}WorkflowStep.Step1Name => true, // First step always active
            _ => ctx.{Entity}ID > 0 // Other steps require entity to exist
        };
    }

    private static bool IsStepComplete({Entity}WorkflowContext ctx, {Entity}WorkflowStep step)
    {
        return step switch
        {
            {Entity}WorkflowStep.Step1Name => true, // Always complete after save
            {Entity}WorkflowStep.Step2Name => ctx.SomeCount > 0,
            // ... step-specific completion logic
            _ => throw new ArgumentOutOfRangeException(nameof(step))
        };
    }

    // Internal context: holds all data needed to compute step states
    internal sealed class {Entity}WorkflowContext
    {
        public int {Entity}ID { get; init; }
        public string? Name { get; init; }
        // ... counts and flags for step completion checks
    }

    internal static async Task<{Entity}WorkflowContext?> LoadWorkflowContextAsync(
        {DbContext} dbContext,
        int {entity}ID)
    {
        return await dbContext.{Entities}
            .AsNoTracking()
            .Where(x => x.{Entity}ID == {entity}ID)
            .Select(x => new {Entity}WorkflowContext
            {
                {Entity}ID = x.{Entity}ID,
                Name = x.Name,
                // ... project only what's needed for step computations
            })
            .SingleOrDefaultAsync();
    }

    // DTO for API response
    public class {Entity}WorkflowProgressDto
    {
        public int {Entity}ID { get; set; }
        public string? Name { get; set; }
        // ... other header fields
        public Dictionary<{Entity}WorkflowStep, WorkflowStepStatus> Steps { get; set; } = new();
    }
}
```

---

## 3. Backend: Step DTOs

Location: `{ModelsProject}/DataTransferObjects/{Entity}/Workflow/`

Create DTOs for each step that needs data beyond the main entity:

### Naming Conventions

- `{Entity}{StepName}StepResponseDto` - GET response with current step data
- `{Entity}{StepName}RequestDto` - POST/PUT request for step save
- Simple form steps can reuse existing `{Entity}UpsertRequestDto`

### Example: File Upload Step DTO

```csharp
public class {Entity}FileUploadRequestDto
{
    public IFormFile File { get; set; } = null!;
    public string? Description { get; set; }
}

public class {Entity}FileUploadResponseDto
{
    public int {Entity}ID { get; set; }
    public string? FileName { get; set; }
    public DateTime? UploadDate { get; set; }
    public bool HasFile => !string.IsNullOrEmpty(FileName);
}
```

---

## 4. Backend: Controller Workflow Endpoints

Add workflow endpoints to `{ApiProject}/Controllers/{Entity}Controller.cs`:

```csharp
#region Workflow

// Progress endpoint - returns step completion/disabled states
[HttpGet("{id}/workflow/progress")]
public async Task<ActionResult<{Entity}WorkflowProgressDto>> GetWorkflowProgress([FromRoute] int id)
{
    var progress = await {Entity}WorkflowProgress.GetProgressAsync(DbContext, id);
    if (progress == null) return NotFound();
    return Ok(progress);
}

// Step GET - returns current data for a step
[HttpGet("{id}/workflow/steps/{step-name}")]
public async Task<ActionResult<StepResponseDto>> Get{StepName}([FromRoute] int id)
{
    var response = await {Entities}.Get{StepName}StepAsync(DbContext, id);
    if (response == null) return NotFound();
    return Ok(response);
}

// Step POST/PUT - saves step data
[HttpPut("{id}/workflow/steps/{step-name}")]
public async Task<IActionResult> Save{StepName}([FromRoute] int id, [FromBody] StepRequestDto request)
{
    var entity = await DbContext.{Entities}.FindAsync(id);
    if (entity == null) return NotFound();

    await {Entities}.Update{StepName}Async(DbContext, entity, request);
    return NoContent();
}

// File upload step
[HttpPost("{id}/workflow/steps/{step-name}/upload")]
public async Task<ActionResult<StepResponseDto>> Upload{StepName}(
    [FromRoute] int id,
    [FromForm] FileUploadRequestDto request)
{
    var entity = await DbContext.{Entities}.FindAsync(id);
    if (entity == null) return NotFound();

    var response = await {Entities}.Process{StepName}UploadAsync(DbContext, entity, request);
    return Ok(response);
}

// Submit workflow
[HttpPost("{id}/workflow/submit")]
public async Task<IActionResult> SubmitWorkflow([FromRoute] int id)
{
    var canSubmit = await {Entity}WorkflowProgress.CanSubmitAsync(DbContext, id);
    if (!canSubmit) return BadRequest("Not all steps are complete.");

    var entity = await DbContext.{Entities}.FindAsync(id);
    if (entity == null) return NotFound();

    await {Entities}.SubmitWorkflowAsync(DbContext, entity);
    return NoContent();
}

#endregion
```

---

## 5. Backend: State Transitions

Workflows typically have a status lifecycle (Draft → Pending → Approved, etc.). Implement state transitions with validation, history tracking, and notifications.

### 5.1 Status Enum

The status enum is typically a lookup table. Common statuses:

| Status | Description |
|--------|-------------|
| Draft | Initial state, workflow in progress |
| Pending | Submitted for review |
| Returned | Returned to submitter for changes |
| Approved | Approved by reviewer |
| Expired | Past validity period |
| WithdrawalRequested | User requested withdrawal |
| Withdrawn | Withdrawal approved |
| UpdateRequested | User requested to make changes |
| UpdateInProgress | Update request approved, changes allowed |

### 5.2 State Transition Methods

Add to `{EFModelsProject}/Entities/{Entities}.StaticHelpers.cs`:

```csharp
#region State Transitions

public static async Task Submit({DbContext} dbContext, int {entity}ID, PersonDetail person)
{
    var entity = await dbContext.{Entities}.FindAsync({entity}ID);
    var canSubmit = await {Entity}WorkflowProgress.CanSubmitAsync(dbContext, {entity}ID);
    if (canSubmit)
    {
        await SetNewStatusAndLogTransition(dbContext, entity, person.PersonID,
            {Entity}StatusEnum.Pending);
        // Optional: Send notification
        // Notification{Entity}.SendSubmittedMessage(entity);
    }
}

public static async Task Return({DbContext} dbContext, int {entity}ID, PersonDetail person)
{
    var entity = await dbContext.{Entities}.FindAsync({entity}ID);
    Check.Require(entity.IsPending(), "Only pending items can be returned.");
    await SetNewStatusAndLogTransition(dbContext, entity, person.PersonID,
        {Entity}StatusEnum.Returned);
}

public static async Task Approve({DbContext} dbContext, int {entity}ID, PersonDetail person)
{
    var entity = await dbContext.{Entities}.FindAsync({entity}ID);
    Check.Require(entity.IsPending(), "Only pending items can be approved.");
    await SetNewStatusAndLogTransition(dbContext, entity, person.PersonID,
        {Entity}StatusEnum.Approved);
}

public static async Task Expire({DbContext} dbContext, int {entity}ID, PersonDetail person)
{
    var entity = await dbContext.{Entities}.FindAsync({entity}ID);
    Check.Require(entity.IsApproved(), "Only approved items can be expired.");
    await SetNewStatusAndLogTransition(dbContext, entity, person.PersonID,
        {Entity}StatusEnum.Expired);
}

public static async Task RequestWithdrawal({DbContext} dbContext, int {entity}ID, PersonDetail person)
{
    var entity = await dbContext.{Entities}.FindAsync({entity}ID);
    Check.Require(entity.IsApproved(), "Only approved items can request withdrawal.");
    await SetNewStatusAndLogTransition(dbContext, entity, person.PersonID,
        {Entity}StatusEnum.WithdrawalRequested);
}

public static async Task ApproveWithdrawal({DbContext} dbContext, int {entity}ID, PersonDetail person)
{
    var entity = await dbContext.{Entities}.FindAsync({entity}ID);
    Check.Require(entity.IsWithdrawalRequested(), "Only items with withdrawal request can be withdrawn.");
    await SetNewStatusAndLogTransition(dbContext, entity, person.PersonID,
        {Entity}StatusEnum.Withdrawn);
}

public static async Task RejectWithdrawal({DbContext} dbContext, int {entity}ID, PersonDetail person)
{
    var entity = await dbContext.{Entities}.FindAsync({entity}ID);
    Check.Require(entity.IsWithdrawalRequested(), "Only items with withdrawal request can be rejected.");
    entity.WithdrawRequestRationale = null;
    await SetNewStatusAndLogTransition(dbContext, entity, person.PersonID,
        {Entity}StatusEnum.Approved);
}

public static async Task RequestToUpdate({DbContext} dbContext, int {entity}ID, PersonDetail person)
{
    var entity = await dbContext.{Entities}.FindAsync({entity}ID);
    Check.Require(entity.IsApproved(), "Only approved items can request updates.");
    await SetNewStatusAndLogTransition(dbContext, entity, person.PersonID,
        {Entity}StatusEnum.UpdateRequested);
}

public static async Task ApproveUpdateRequest({DbContext} dbContext, int {entity}ID, PersonDetail person)
{
    var entity = await dbContext.{Entities}.FindAsync({entity}ID);
    Check.Require(entity.IsUpdateRequested(), "Only items with update request can be approved.");
    await SetNewStatusAndLogTransition(dbContext, entity, person.PersonID,
        {Entity}StatusEnum.UpdateInProgress);
}

public static async Task RejectUpdateRequest({DbContext} dbContext, int {entity}ID, PersonDetail person)
{
    var entity = await dbContext.{Entities}.FindAsync({entity}ID);
    Check.Require(entity.IsUpdateRequested(), "Only items with update request can be rejected.");
    await SetNewStatusAndLogTransition(dbContext, entity, person.PersonID,
        {Entity}StatusEnum.Approved);
}

#endregion

#region History Tracking

public static async Task SetNewStatusAndLogTransition(
    {DbContext} dbContext,
    {Entity}? entity,
    int personID,
    {Entity}StatusEnum newStatus)
{
    if (entity == null) return;

    var newStatusID = (int)newStatus;
    if (entity.{Entity}StatusID != newStatusID)
    {
        entity.{Entity}StatusID = newStatusID;

        var history = new {Entity}History
        {
            {Entity}ID = entity.{Entity}ID,
            {Entity}StatusID = newStatusID,
            UpdatePersonID = personID,
            TransitionDate = DateTime.UtcNow
        };
        dbContext.{Entity}Histories.Add(history);

        await dbContext.SaveChangesAsync();
    }
}

#endregion
```

### 5.3 Status Helper Extension Methods

Add to `{EFModelsProject}/Entities/{Entity}.cs` (partial class):

```csharp
public partial class {Entity}
{
    public bool IsDraft() => {Entity}StatusID == (int){Entity}StatusEnum.Draft;
    public bool IsPending() => {Entity}StatusID == (int){Entity}StatusEnum.Pending;
    public bool IsReturned() => {Entity}StatusID == (int){Entity}StatusEnum.Returned;
    public bool IsApproved() => {Entity}StatusID == (int){Entity}StatusEnum.Approved;
    public bool IsExpired() => {Entity}StatusID == (int){Entity}StatusEnum.Expired;
    public bool IsWithdrawalRequested() => {Entity}StatusID == (int){Entity}StatusEnum.WithdrawalRequested;
    public bool IsWithdrawn() => {Entity}StatusID == (int){Entity}StatusEnum.Withdrawn;
    public bool IsUpdateRequested() => {Entity}StatusID == (int){Entity}StatusEnum.UpdateRequested;
    public bool IsUpdateInProgress() => {Entity}StatusID == (int){Entity}StatusEnum.UpdateInProgress;

    // Can edit if draft, returned, or update in progress
    public bool CanEdit() => IsDraft() || IsReturned() || IsUpdateInProgress();
}
```

### 5.4 Controller State Transition Endpoints

Add to `{ApiProject}/Controllers/{Entity}Controller.cs`:

```csharp
#region State Transitions

[HttpPost("{id}/workflow/submit")]
[AdminFeature]
[EntityNotFound(typeof({Entity}), "{entity}ID")]
public async Task<IActionResult> Submit([FromRoute] int id)
{
    await {Entities}.Submit(DbContext, id, CallingUser);
    return Ok();
}

[HttpPost("{id}/workflow/return")]
[AdminFeature]
[EntityNotFound(typeof({Entity}), "{entity}ID")]
public async Task<IActionResult> Return([FromRoute] int id)
{
    await {Entities}.Return(DbContext, id, CallingUser);
    return Ok();
}

[HttpPost("{id}/workflow/approve")]
[AdminFeature]
[EntityNotFound(typeof({Entity}), "{entity}ID")]
public async Task<IActionResult> Approve([FromRoute] int id)
{
    await {Entities}.Approve(DbContext, id, CallingUser);
    return Ok();
}

[HttpPost("{id}/workflow/request-withdrawal")]
[AdminFeature]
[EntityNotFound(typeof({Entity}), "{entity}ID")]
public async Task<IActionResult> RequestWithdrawal([FromRoute] int id)
{
    await {Entities}.RequestWithdrawal(DbContext, id, CallingUser);
    return Ok();
}

[HttpPost("{id}/workflow/approve-withdrawal")]
[AdminFeature]
[EntityNotFound(typeof({Entity}), "{entity}ID")]
public async Task<IActionResult> ApproveWithdrawal([FromRoute] int id)
{
    await {Entities}.ApproveWithdrawal(DbContext, id, CallingUser);
    return Ok();
}

[HttpPost("{id}/workflow/reject-withdrawal")]
[AdminFeature]
[EntityNotFound(typeof({Entity}), "{entity}ID")]
public async Task<IActionResult> RejectWithdrawal([FromRoute] int id)
{
    await {Entities}.RejectWithdrawal(DbContext, id, CallingUser);
    return Ok();
}

[HttpPost("{id}/workflow/request-update")]
[AdminFeature]
[EntityNotFound(typeof({Entity}), "{entity}ID")]
public async Task<IActionResult> RequestToUpdate([FromRoute] int id)
{
    await {Entities}.RequestToUpdate(DbContext, id, CallingUser);
    return Ok();
}

[HttpPost("{id}/workflow/approve-update")]
[AdminFeature]
[EntityNotFound(typeof({Entity}), "{entity}ID")]
public async Task<IActionResult> ApproveUpdateRequest([FromRoute] int id)
{
    await {Entities}.ApproveUpdateRequest(DbContext, id, CallingUser);
    return Ok();
}

[HttpPost("{id}/workflow/reject-update")]
[AdminFeature]
[EntityNotFound(typeof({Entity}), "{entity}ID")]
public async Task<IActionResult> RejectUpdateRequest([FromRoute] int id)
{
    await {Entities}.RejectUpdateRequest(DbContext, id, CallingUser);
    return Ok();
}

#endregion

#region History & Review Comments

[HttpGet("{id}/history")]
[EntityNotFound(typeof({Entity}), "{entity}ID")]
public async Task<ActionResult<IEnumerable<{Entity}HistoryDetail>>> ListHistory([FromRoute] int id)
{
    var items = await {Entity}Histories.ListBy{Entity}AsDetailAsync(DbContext, id);
    return Ok(items);
}

[HttpGet("{id}/review-comments")]
[EntityNotFound(typeof({Entity}), "{entity}ID")]
public async Task<ActionResult<IEnumerable<{Entity}ReviewCommentDetail>>> ListReviewComments([FromRoute] int id)
{
    var items = await {Entity}ReviewComments.ListBy{Entity}AsDetailAsync(DbContext, id);
    return Ok(items);
}

[HttpPost("{id}/review-comments")]
[AdminFeature]
[EntityNotFound(typeof({Entity}), "{entity}ID")]
public async Task<ActionResult<{Entity}ReviewCommentDetail>> CreateReviewComment(
    [FromRoute] int id,
    [FromBody] {Entity}ReviewCommentRequest dto)
{
    if (dto == null || string.IsNullOrWhiteSpace(dto.ReviewCommentNote))
    {
        return BadRequest("Review comment is required.");
    }
    var created = await {Entity}ReviewComments.CreateAsync(DbContext, id, CallingUser.PersonID, dto);
    return Ok(created);
}

[HttpDelete("{id}/review-comments/{reviewCommentID}")]
[AdminFeature]
[EntityNotFound(typeof({Entity}), "{entity}ID")]
public async Task<IActionResult> DeleteReviewComment([FromRoute] int id, [FromRoute] int reviewCommentID)
{
    var ok = await {Entity}ReviewComments.DeleteAsync(DbContext, id, reviewCommentID);
    if (!ok) return NotFound();
    return NoContent();
}

#endregion
```

### 5.5 State Transition Diagram

```
                    ┌──────────┐
                    │  Draft   │
                    └────┬─────┘
                         │ Submit
                         ▼
                    ┌──────────┐
           ┌────────│ Pending  │────────┐
           │        └────┬─────┘        │
           │ Return      │ Approve      │
           ▼             ▼              │
    ┌──────────┐   ┌──────────┐         │
    │ Returned │   │ Approved │◄────────┘
    └────┬─────┘   └────┬─────┘
         │              │
         │ Submit       ├─── RequestWithdrawal ──► WithdrawalRequested
         │              │                              │
         └──────────────┤                    ┌─────────┴─────────┐
                        │                    │                   │
                        │              ApproveWithdrawal   RejectWithdrawal
                        │                    │                   │
                        │                    ▼                   │
                        │              ┌──────────┐              │
                        │              │ Withdrawn│              │
                        │              └──────────┘              │
                        │                                        │
                        ├─── RequestToUpdate ────► UpdateRequested
                        │                              │
                        │                    ┌─────────┴─────────┐
                        │                    │                   │
                        │              ApproveUpdate       RejectUpdate
                        │                    │                   │
                        │                    ▼                   │
                        │              ┌───────────────┐         │
                        │              │UpdateInProgress│────────┤
                        │              └───────┬───────┘         │
                        │                      │ Submit          │
                        │                      └─────────────────┘
                        │
                        └─── Expire ──► Expired
```

---

## 6. Frontend: State Transition Actions

### 6.1 Detail Page Actions

Add action buttons to the detail page based on current status:

```typescript
// In detail component
public {Entity}StatusEnum = {Entity}StatusEnum;

public submitForReview(): void {
    this.confirmService.confirm({
        title: "Submit for Review",
        message: "Are you sure you want to submit this for review?",
        buttonTextYes: "Submit",
        buttonClassYes: "btn-primary"
    }).then(confirmed => {
        if (confirmed) {
            this.{entity}Service.submit{Entity}(this.{entity}ID).subscribe(() => {
                this.alertService.pushAlert(new Alert("Submitted for review.", AlertContext.Success));
                this.refresh();
            });
        }
    });
}

public approve(): void {
    this.confirmService.confirm({
        title: "Approve",
        message: "Are you sure you want to approve this?",
        buttonTextYes: "Approve",
        buttonClassYes: "btn-success"
    }).then(confirmed => {
        if (confirmed) {
            this.{entity}Service.approve{Entity}(this.{entity}ID).subscribe(() => {
                this.alertService.pushAlert(new Alert("Approved.", AlertContext.Success));
                this.refresh();
            });
        }
    });
}

public return(): void {
    this.confirmService.confirm({
        title: "Return for Changes",
        message: "Are you sure you want to return this for changes?",
        buttonTextYes: "Return",
        buttonClassYes: "btn-warning"
    }).then(confirmed => {
        if (confirmed) {
            this.{entity}Service.return{Entity}(this.{entity}ID).subscribe(() => {
                this.alertService.pushAlert(new Alert("Returned for changes.", AlertContext.Success));
                this.refresh();
            });
        }
    });
}
```

### 6.2 Detail Page Template Actions

```html
@if ({entity}$ | async; as {entity}) {
    <div class="action-buttons">
        <!-- Draft/Returned: Show Edit and Submit -->
        @if ({entity}.Status.{Entity}StatusID === {Entity}StatusEnum.Draft ||
             {entity}.Status.{Entity}StatusID === {Entity}StatusEnum.Returned) {
            <a [routerLink]="['/{entities}/edit', {entity}.{Entity}ID]" class="btn btn-secondary">
                <icon icon="Edit"></icon> Edit
            </a>
            <button class="btn btn-primary" (click)="submitForReview()">
                <icon icon="Check"></icon> Submit for Review
            </button>
        }

        <!-- Pending: Show Approve/Return (for reviewers) -->
        @if ({entity}.Status.{Entity}StatusID === {Entity}StatusEnum.Pending) {
            <button class="btn btn-success" (click)="approve()">
                <icon icon="Check"></icon> Approve
            </button>
            <button class="btn btn-warning" (click)="return()">
                <icon icon="Undo"></icon> Return for Changes
            </button>
        }

        <!-- Approved: Show Request Withdrawal/Update -->
        @if ({entity}.Status.{Entity}StatusID === {Entity}StatusEnum.Approved) {
            <button class="btn btn-secondary" (click)="requestWithdrawal()">
                Request Withdrawal
            </button>
            <button class="btn btn-secondary" (click)="requestUpdate()">
                Request to Update
            </button>
        }

        <!-- WithdrawalRequested: Show Approve/Reject (for reviewers) -->
        @if ({entity}.Status.{Entity}StatusID === {Entity}StatusEnum.WithdrawalRequested) {
            <button class="btn btn-danger" (click)="approveWithdrawal()">
                Approve Withdrawal
            </button>
            <button class="btn btn-secondary" (click)="rejectWithdrawal()">
                Reject Withdrawal
            </button>
        }

        <!-- UpdateRequested: Show Approve/Reject (for reviewers) -->
        @if ({entity}.Status.{Entity}StatusID === {Entity}StatusEnum.UpdateRequested) {
            <button class="btn btn-primary" (click)="approveUpdate()">
                Approve Update Request
            </button>
            <button class="btn btn-secondary" (click)="rejectUpdate()">
                Reject Update Request
            </button>
        }

        <!-- UpdateInProgress: Show Edit and Submit -->
        @if ({entity}.Status.{Entity}StatusID === {Entity}StatusEnum.UpdateInProgress) {
            <a [routerLink]="['/{entities}/edit', {entity}.{Entity}ID]" class="btn btn-secondary">
                <icon icon="Edit"></icon> Edit
            </a>
            <button class="btn btn-primary" (click)="submitForReview()">
                <icon icon="Check"></icon> Submit Updates
            </button>
        }
    </div>
}
```

### 6.3 History and Review Comments Grid

```typescript
// History grid columns
this.historyColumnDefs = [
    this.utilityFunctionsService.createDateColumnDef("Date", "TransitionDate", "short"),
    this.utilityFunctionsService.createBasicColumnDef("Status", "{Entity}Status.{Entity}StatusName"),
    this.utilityFunctionsService.createLinkColumnDef("Updated By", "UpdatePerson.FullName", "UpdatePerson.PersonID", {
        InRouterLink: "/users/",
    }),
];

// Review comments (with add/delete capability)
public reviewComments$: Observable<{Entity}ReviewCommentDetail[]>;
public newCommentText: string = "";
public isAddCommentOpen: boolean = false;

public submitReviewComment(): void {
    if (!this.newCommentText) return;
    const req = { ReviewCommentNote: this.newCommentText };
    this.{entity}Service.createReviewComment{Entity}(this.{entity}ID, req).subscribe({
        next: () => {
            this.refreshComments();
            this.closeAddComment();
        }
    });
}

public deleteReviewComment(comment: {Entity}ReviewCommentDetail): void {
    this.confirmService.confirm({
        title: "Delete Comment",
        message: "Delete this review comment?",
        buttonTextYes: "Delete",
        buttonClassYes: "btn-danger"
    }).then(confirmed => {
        if (confirmed) {
            this.{entity}Service.deleteReviewComment{Entity}(this.{entity}ID, comment.ReviewCommentID).subscribe(() => {
                this.refreshComments();
            });
        }
    });
}
```

---

## 7. Generate TypeScript

After API code is complete:

```powershell
# Build the API to generate swagger.json
dotnet build {ApiProject}

# Generate TypeScript models
cd {FrontendProject}
npm run gen-model
```

Verify generated files include:
- `{entity}-workflow-progress-dto.ts`
- `{entity}-status-enum.ts`
- Step DTOs and enums
- Service methods for workflow and state transition endpoints

---

## 8. Frontend: Workflow Progress Service

Create `{FrontendProject}/src/app/services/{entity}-workflow-progress.service.ts`:

```typescript
import { Injectable } from "@angular/core";
import { Subject, ReplaySubject, Observable, Subscription } from "rxjs";
import { {Entity}Service } from "../shared/generated/api/{entity}.service";
import { {Entity}WorkflowProgressDto } from "../shared/generated/model/{entity}-workflow-progress-dto";

@Injectable({
    providedIn: "root",
})
export class {Entity}WorkflowProgressService {
    private progressSubject: Subject<{Entity}WorkflowProgressDto> = new ReplaySubject();
    public progressObservable$: Observable<{Entity}WorkflowProgressDto> = this.progressSubject.asObservable();

    private progressSubscription = Subscription.EMPTY;

    constructor(private {entity}Service: {Entity}Service) {}

    updateProgress({entity}ID: number): void {
        this.progressSubscription.unsubscribe();
        this.getProgress({entity}ID);
    }

    getProgress({entity}ID: number | null): void {
        if ({entity}ID) {
            // Fetch from API
            this.progressSubscription = this.{entity}Service.getWorkflowProgress{Entity}({entity}ID).subscribe({
                next: (dto) => this.progressSubject.next(dto),
            });
        } else {
            // No ID: emit client-side defaults (first step active only)
            this.progressSubject.next({
                Steps: {
                    Step1Name: { Completed: false, Disabled: false },
                    Step2Name: { Completed: false, Disabled: true },
                    Step3Name: { Completed: false, Disabled: true },
                    // ... all steps disabled except first
                },
            });
        }
    }
}
```

---

## 9. Frontend: Workflow Outlet Component

Create `{FrontendProject}/src/app/areas/{area}/{entities}/workflow/{entity}-workflow-outlet/`:

### Component TypeScript

```typescript
import { Component, Input, OnInit } from "@angular/core";
import { Router, RouterLink, RouterOutlet } from "@angular/router";
import { Observable, tap } from "rxjs";
import { {Entity}WorkflowProgressService } from "src/app/services/{entity}-workflow-progress.service";
import { AsyncPipe } from "@angular/common";
import { WorkflowNavComponent } from "src/app/shared/components/workflow-nav/workflow-nav.component";
import { WorkflowNavItemComponent } from "src/app/shared/components/workflow-nav/workflow-nav-item/workflow-nav-item.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { {Entity}Service } from "src/app/shared/generated/api/{entity}.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { {Entity}WorkflowProgressDto } from "src/app/shared/generated/model/{entity}-workflow-progress-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

@Component({
    selector: "{entity}-workflow-outlet",
    standalone: true,
    imports: [RouterOutlet, RouterLink, AsyncPipe, WorkflowNavComponent, WorkflowNavItemComponent, IconComponent],
    templateUrl: "./{entity}-workflow-outlet.component.html",
    styleUrls: ["./{entity}-workflow-outlet.component.scss"],
})
export class {Entity}WorkflowOutletComponent implements OnInit {
    public progress$: Observable<{Entity}WorkflowProgressDto>;
    @Input() {entity}ID: number | null = null;

    constructor(
        private router: Router,
        private confirmService: ConfirmService,
        private alertService: AlertService,
        private progressService: {Entity}WorkflowProgressService,
        private {entity}Service: {Entity}Service
    ) {}

    ngOnInit() {
        this.progress$ = this.progressService.progressObservable$;
        this.progressService.getProgress(this.{entity}ID);
    }

    public delete{Entity}({entity}ID: number, name: string): void {
        const modalContents = `<p>Are you sure you want to delete '${name}'?</p>`;
        this.confirmService
            .confirm({
                buttonClassYes: "btn-danger",
                buttonTextYes: "Delete",
                buttonTextNo: "Cancel",
                title: "Delete {Entity}",
                message: modalContents
            })
            .then((confirmed) => {
                if (confirmed) {
                    this.{entity}Service.delete{Entity}({entity}ID).subscribe(() => {
                        this.alertService.clearAlerts();
                        this.alertService.pushAlert(new Alert("Successfully deleted.", AlertContext.Success));
                        this.router.navigate(["/{entities}"]);
                    });
                }
            });
    }
}
```

### Component HTML Template

```html
<div class="workflow dashboard">
    <aside class="sidebar">
        @if (progress$ | async; as progressDto) {
            <div class="sidebar-header">
                <h5 class="sidebar-title">
                    <div>
                        <p class="registration-label">
                            {Entity Title}
                            @if (progressDto?.{Entity}ID) {
                                <a
                                    class="ml-2 delete"
                                    title="Delete {Entity}"
                                    (click)="delete{Entity}(progressDto.{Entity}ID, progressDto.Name)">
                                    <icon icon="Delete"></icon>
                                </a>
                            }
                        </p>
                        @if (progressDto?.{Entity}ID) {
                            <p>
                                <a class="assessment-link" [routerLink]="['/{entities}', progressDto.{Entity}ID]">
                                    <strong>{{ progressDto.Name }}</strong>
                                </a>
                            </p>
                        } @else {
                            <p><strong>New</strong></p>
                        }
                    </div>
                </h5>
            </div>
            <workflow-nav>
                <workflow-nav-item
                    [navRouterLink]="['step-1']"
                    [complete]="progressDto?.Steps?.Step1Name?.Completed"
                    [disabled]="progressDto?.Steps?.Step1Name?.Disabled"
                    >Step 1 Title</workflow-nav-item>
                <workflow-nav-item
                    [navRouterLink]="['step-2']"
                    [complete]="progressDto?.Steps?.Step2Name?.Completed"
                    [disabled]="progressDto?.Steps?.Step2Name?.Disabled"
                    >Step 2 Title</workflow-nav-item>
                <!-- ... remaining steps -->
            </workflow-nav>
        }
    </aside>
    <main class="main">
        <div class="outlet-container">
            <router-outlet></router-outlet>
        </div>
    </main>
</div>
```

### Component SCSS

```scss
@use "src/styles/variables" as *;

.workflow {
    display: grid;
    grid-template-columns: 300px 1fr;
    min-height: calc(100vh - 100px);
}

.sidebar {
    background-color: $gray-100;
    border-right: 1px solid $gray-300;
    padding: 1rem;
}

.sidebar-header {
    margin-bottom: 1rem;
}

.sidebar-title {
    margin: 0;
}

.registration-label {
    font-size: 0.875rem;
    color: $gray-600;
    margin-bottom: 0.25rem;
}

.delete {
    cursor: pointer;
    color: $danger;
}

.main {
    padding: 1rem 2rem;
}

.outlet-container {
    max-width: 1200px;
}
```

---

## 10. Frontend: Step Components

Create a component for each step in `{FrontendProject}/src/app/areas/{area}/{entities}/workflow/{step-name}/`:

### Form Entry Step Template

```typescript
import { Component, Input, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { {Entity}WorkflowProgressService } from "src/app/services/{entity}-workflow-progress.service";
import { {Entity}Service } from "src/app/shared/generated/api/{entity}.service";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WorkflowBodyComponent } from "src/app/shared/components/workflow-body/workflow-body.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { FormGroup, ReactiveFormsModule } from "@angular/forms";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { AsyncPipe } from "@angular/common";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
// Import generated form controls
import { {Entity}UpsertRequestForm, {Entity}UpsertRequestFormControls } from "src/app/shared/generated/model/{entity}-upsert-request";

@Component({
    selector: "{entity}-{step-name}",
    standalone: true,
    templateUrl: "./{step-name}.component.html",
    styleUrls: ["./{step-name}.component.scss"],
    imports: [
        AsyncPipe,
        ReactiveFormsModule,
        PageHeaderComponent,
        WorkflowBodyComponent,
        AlertDisplayComponent,
        FormFieldComponent
    ],
})
export class {StepName}Component implements OnInit {
    @Input() {entity}ID: number | null = null;
    public isLoadingSubmit = false;
    public FormFieldType = FormFieldType;

    public formGroup: FormGroup<{Entity}UpsertRequestForm> = new FormGroup<{Entity}UpsertRequestForm>({
        // Use generated form controls
        FieldName: {Entity}UpsertRequestFormControls.FieldName(),
    });

    constructor(
        private progress: {Entity}WorkflowProgressService,
        private alertService: AlertService,
        private router: Router,
        private {entity}Service: {Entity}Service
    ) {}

    ngOnInit(): void {
        if (this.{entity}ID) {
            // Edit flow: load existing data
            this.{entity}Service.get{Entity}(this.{entity}ID).subscribe((detail) => {
                this.formGroup.patchValue(detail);
            });
        }
    }

    public save(andContinue: boolean = false): void {
        this.isLoadingSubmit = true;

        const request = this.formGroup.value;

        if (this.{entity}ID) {
            // Update existing
            this.{entity}Service.update{Entity}(this.{entity}ID, request).subscribe({
                next: () => {
                    this.handleSaveSuccess(this.{entity}ID!, andContinue);
                },
                error: () => {
                    this.isLoadingSubmit = false;
                }
            });
        } else {
            // Create new
            this.{entity}Service.create{Entity}(request).subscribe({
                next: (response) => {
                    this.{entity}ID = response.{Entity}ID;
                    // Navigate to edit route for subsequent steps
                    this.router.navigate(["/{entities}/edit", this.{entity}ID]);
                    this.handleSaveSuccess(this.{entity}ID!, andContinue);
                },
                error: () => {
                    this.isLoadingSubmit = false;
                }
            });
        }
    }

    private handleSaveSuccess(id: number, andContinue: boolean): void {
        this.isLoadingSubmit = false;
        this.alertService.clearAlerts();
        this.alertService.pushAlert(new Alert("Saved successfully.", AlertContext.Success));
        this.progress.updateProgress(id);

        if (andContinue) {
            this.router.navigate(["/{entities}/edit", id, "next-step"]);
        }
    }
}
```

### Form Entry Step HTML

```html
<page-header pageTitle="{Step Title}"></page-header>
<workflow-body>
    <app-alert-display></app-alert-display>

    <div class="card">
        <div class="card-header">
            <span class="card-title">{Section Title}</span>
        </div>
        <div class="card-body">
            <form [formGroup]="formGroup">
                <div class="grid-12">
                    <div class="g-col-6">
                        <form-field
                            [formGroup]="formGroup"
                            fieldLabel="Field Label"
                            formControlName="FieldName"
                            [type]="FormFieldType.Text">
                        </form-field>
                    </div>
                </div>
            </form>
        </div>
    </div>
</workflow-body>

<div class="page-footer">
    <button class="btn btn-secondary" (click)="save(false)" [disabled]="isLoadingSubmit || formGroup.invalid">
        Save
    </button>
    <button class="btn btn-primary" (click)="save(true)" [disabled]="isLoadingSubmit || formGroup.invalid">
        Save &amp; Continue
    </button>
</div>
```

### Map Selection Step (see /migrate-map for details)

For steps involving map selection, combine the workflow step pattern with map components.

### File Upload Step

For file upload steps, use `[FromForm]` on the backend and `FormData` on the frontend.

### Grid/List Management Step (see /migrate-grid for details)

For steps managing collections, use grid components with add/remove actions.

---

## 11. Frontend: Route Configuration

Add routes to `{FrontendProject}/src/app/app.routes.ts`:

```typescript
// Create flow - only first step available until entity exists
{
    path: "{entities}/new",
    title: "New {Entity}",
    loadComponent: () =>
        import("./areas/{area}/{entities}/workflow/{entity}-workflow-outlet/{entity}-workflow-outlet.component").then(
            (m) => m.{Entity}WorkflowOutletComponent
        ),
    children: [
        { path: "", redirectTo: "step-1", pathMatch: "full" },
        {
            path: "step-1",
            loadComponent: () =>
                import("./areas/{area}/{entities}/workflow/step-1/step-1.component").then(
                    (m) => m.Step1Component
                ),
        },
    ],
},

// Edit flow - all steps available
{
    path: "{entities}/edit/:{entity}ID",
    title: "Edit {Entity}",
    loadComponent: () =>
        import("./areas/{area}/{entities}/workflow/{entity}-workflow-outlet/{entity}-workflow-outlet.component").then(
            (m) => m.{Entity}WorkflowOutletComponent
        ),
    children: [
        { path: "", redirectTo: "step-1", pathMatch: "full" },
        {
            path: "step-1",
            loadComponent: () =>
                import("./areas/{area}/{entities}/workflow/step-1/step-1.component").then(
                    (m) => m.Step1Component
                ),
        },
        {
            path: "step-2",
            loadComponent: () =>
                import("./areas/{area}/{entities}/workflow/step-2/step-2.component").then(
                    (m) => m.Step2Component
                ),
        },
        // ... all remaining steps
    ],
},
```

---

## 12. Checklist Before Completion

### Backend: Workflow Steps

- [ ] Created `{Entity}WorkflowProgress.cs` with step enum and completion logic
- [ ] Created step DTOs in `Workflow/` subfolder
- [ ] Added `GetWorkflowProgress` endpoint
- [ ] Added GET/POST/PUT endpoints for each step
- [ ] Built API and regenerated swagger

### Backend: State Transitions

- [ ] Created status helper methods (`IsDraft()`, `IsPending()`, etc.)
- [ ] Created state transition methods (`Submit`, `Approve`, `Return`, etc.)
- [ ] Created `SetNewStatusAndLogTransition` for history tracking
- [ ] Created `{Entity}History` table and DTOs
- [ ] Created `{Entity}ReviewComment` table and DTOs (if needed)
- [ ] Added state transition endpoints to controller
- [ ] Added history and review comment endpoints

### Frontend: Workflow

- [ ] Created `{Entity}WorkflowProgressService`
- [ ] Created `{Entity}WorkflowOutletComponent` with sidebar and nav
- [ ] Created step components (one per step)
- [ ] Added create route (`/new`) with first step only
- [ ] Added edit route (`/edit/:id`) with all step children
- [ ] Regenerated TypeScript models (`npm run gen-model`)

### Frontend: State Transitions

- [ ] Added action buttons to detail page based on status
- [ ] Implemented state transition handlers (`submitForReview()`, `approve()`, etc.)
- [ ] Added history grid with transition log
- [ ] Added review comments section (if applicable)
- [ ] Status-based UI (edit button only when editable, etc.)

### Testing

- [ ] Progress service correctly computes step states
- [ ] Navigation between steps works
- [ ] Save and continue advances to next step
- [ ] First step creates entity and redirects to edit route
- [ ] Disabled steps prevent navigation
- [ ] Delete action works from sidebar
- [ ] State transitions work correctly
- [ ] History is logged for each transition
- [ ] Review comments can be added/deleted

### Code Quality

- [ ] No Bootstrap classes (use grid-12, card system)
- [ ] All queries use `.AsNoTracking().Select()`
- [ ] Route params use `@Input()` binding
- [ ] Components are standalone with explicit imports
- [ ] State validation before transitions (Check.Require)

---

## Cross-References

| If you're also doing... | Load |
|-------------------------|------|
| Creating data grids in steps | `/migrate-grid` |
| Creating maps in steps | `/migrate-map` |
| Creating CRUD modals in steps | `/crud-modal` |
| Writing tests | `/write-tests` |
