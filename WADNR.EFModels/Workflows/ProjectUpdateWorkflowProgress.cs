using Microsoft.EntityFrameworkCore;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Workflows;

/// <summary>
/// Workflow progress computation for the Project Update wizard.
/// Determines step completion states and whether an update batch can be submitted for approval.
/// </summary>
public static class ProjectUpdateWorkflowProgress
{
    /// <summary>
    /// All steps in the Project Update wizard, in order.
    /// </summary>
    public enum ProjectUpdateWorkflowStep
    {
        Basics = 1,
        LocationSimple = 2,
        LocationDetailed = 3,
        PriorityLandscapes = 4,
        DnrUplandRegions = 5,
        Counties = 6,
        Treatments = 7,
        Contacts = 8,
        Organizations = 9,
        ExpectedFunding = 10,
        Photos = 11,
        ExternalLinks = 12,
        DocumentsNotes = 13
    }

    /// <summary>
    /// Internal context loaded once for computing all step states.
    /// </summary>
    private sealed record ProjectUpdateWorkflowContext
    {
        public int ProjectUpdateBatchID { get; init; }
        public int ProjectID { get; init; }
        public string ProjectName { get; init; } = string.Empty;
        public int ProjectUpdateStateID { get; init; }
        public string ProjectUpdateStateName { get; init; } = string.Empty;
        public DateTime LastUpdateDate { get; init; }
        public string? LastUpdatedByPersonName { get; init; }
        public string? SubmittedByPersonName { get; init; }
        public DateTime? SubmittedDate { get; init; }
        public string? ReturnedByPersonName { get; init; }
        public DateTime? ReturnedDate { get; init; }
        public int ProjectStageID { get; init; }
        public DateOnly? PlannedDate { get; init; }
        public bool HasSimpleLocation { get; init; }
        public string? ProjectLocationNotes { get; init; }
        public int ProjectLocationSimpleTypeID { get; init; }
        public bool HasDetailedLocations { get; init; }
        public bool HasPriorityLandscapes { get; init; }
        public string? NoPriorityLandscapesExplanation { get; init; }
        public bool HasDnrUplandRegions { get; init; }
        public string? NoRegionsExplanation { get; init; }
        public bool HasCounties { get; init; }
        public string? NoCountiesExplanation { get; init; }
        public bool HasTreatments { get; init; }
        public bool HasContacts { get; init; }
        public bool HasOrganizations { get; init; }
        public bool HasLeadImplementer { get; init; }
        public bool HasExpectedFunding { get; init; }
        public bool HasPhotos { get; init; }
        public bool HasExternalLinks { get; init; }
        public bool HasDocuments { get; init; }
        public bool HasNotes { get; init; }

        // Reviewer comment columns from ProjectUpdateBatch
        public string? BasicsComment { get; init; }
        public string? LocationSimpleComment { get; init; }
        public string? LocationDetailedComment { get; init; }
        public string? ExpectedFundingComment { get; init; }
        public string? ContactsComment { get; init; }
        public string? OrganizationsComment { get; init; }
    }

    /// <summary>
    /// Gets the workflow progress for an update batch, including step completion states and submit eligibility.
    /// </summary>
    public static async Task<UpdateWorkflowProgressResponse?> GetProgressAsync(WADNRDbContext dbContext, int projectUpdateBatchID)
    {
        return await GetProgressForUserAsync(dbContext, projectUpdateBatchID, null);
    }

    /// <summary>
    /// Gets the workflow progress for an update batch, including user-specific permission flags.
    /// </summary>
    public static async Task<UpdateWorkflowProgressResponse?> GetProgressForUserAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        PersonDetail? callingUser)
    {
        var ctx = await LoadWorkflowContextAsync(dbContext, projectUpdateBatchID);
        if (ctx == null) return null;

        // Calculate has-changes for each step by comparing update data to project data
        var stepChanges = await GetStepChangesAsync(dbContext, projectUpdateBatchID, ctx.ProjectID);

        var steps = new Dictionary<string, WorkflowStepStatus>();
        foreach (var step in Enum.GetValues<ProjectUpdateWorkflowStep>())
        {
            var stepKey = step.ToString();
            steps[stepKey] = new WorkflowStepStatus
            {
                IsComplete = IsStepComplete(ctx, step),
                IsDisabled = !IsStepActive(ctx, step),
                IsRequired = IsStepRequired(step),
                HasChanges = stepChanges.TryGetValue(stepKey, out var hasChanges) && hasChanges
            };
        }

        var dto = new UpdateWorkflowProgressResponse
        {
            ProjectUpdateBatchID = ctx.ProjectUpdateBatchID,
            ProjectID = ctx.ProjectID,
            ProjectName = ctx.ProjectName,
            ProjectUpdateStateID = ctx.ProjectUpdateStateID,
            ProjectUpdateStateName = ctx.ProjectUpdateStateName,
            LastUpdateDate = ctx.LastUpdateDate,
            LastUpdatedByPersonName = ctx.LastUpdatedByPersonName,
            SubmittedByPersonName = ctx.SubmittedByPersonName,
            SubmittedDate = ctx.SubmittedDate,
            ReturnedByPersonName = ctx.ReturnedByPersonName,
            ReturnedDate = ctx.ReturnedDate,
            CanSubmit = CanSubmit(ctx, steps),
            IsReadyToApprove = IsReadyToApprove(ctx, steps),
            Steps = steps,
            ReviewerComments = ctx.ProjectUpdateStateID == (int)ProjectUpdateStateEnum.Returned
                ? BuildReviewerComments(ctx)
                : null
        };

        // Populate user permission flags
        PopulateUserPermissionFlags(dto, ctx, callingUser, dbContext);

        return dto;
    }

    /// <summary>
    /// Populates user permission flags based on calling user's role and batch state.
    /// </summary>
    private static void PopulateUserPermissionFlags(
        UpdateWorkflowProgressResponse dto,
        ProjectUpdateWorkflowContext ctx,
        PersonDetail? callingUser,
        WADNRDbContext dbContext)
    {
        // Default all to false for anonymous/unassigned users
        if (callingUser == null || callingUser.IsAnonymousOrUnassigned())
        {
            dto.CanEdit = false;
            dto.CanApprove = false;
            dto.CanReturn = false;
            dto.CanDelete = false;
            return;
        }

        var isCreatedOrReturned = ctx.ProjectUpdateStateID == (int)ProjectUpdateStateEnum.Created ||
                                   ctx.ProjectUpdateStateID == (int)ProjectUpdateStateEnum.Returned;
        var isSubmitted = ctx.ProjectUpdateStateID == (int)ProjectUpdateStateEnum.Submitted;

        // Approve/Return: only available to approvers when batch is Submitted
        var canApproveProjects = callingUser.HasElevatedProjectAccess() || callingUser.HasCanEditProgramRole();
        dto.CanApprove = canApproveProjects && isSubmitted;
        dto.CanReturn = canApproveProjects && isSubmitted;

        // Edit/Delete: only when in Created or Returned state
        if (callingUser.HasElevatedProjectAccess())
        {
            dto.CanEdit = isCreatedOrReturned;
            dto.CanDelete = isCreatedOrReturned;
        }
        else
        {
            // Check if user's org is associated with the project
            var userOrgID = callingUser.OrganizationID;
            if (userOrgID.HasValue)
            {
                var projectOrgIds = dbContext.ProjectOrganizations
                    .AsNoTracking()
                    .Where(po => po.ProjectID == ctx.ProjectID)
                    .Select(po => po.OrganizationID)
                    .ToList();
                var hasOrgAccess = projectOrgIds.Contains(userOrgID.Value);
                dto.CanEdit = hasOrgAccess && isCreatedOrReturned;
                dto.CanDelete = hasOrgAccess && isCreatedOrReturned;
            }
            else
            {
                dto.CanEdit = false;
                dto.CanDelete = false;
            }
        }
    }

    /// <summary>
    /// Checks if all required steps are complete and the batch can be submitted for approval.
    /// </summary>
    public static async Task<bool> CanSubmitAsync(WADNRDbContext dbContext, int projectUpdateBatchID)
    {
        var progress = await GetProgressAsync(dbContext, projectUpdateBatchID);
        return progress?.CanSubmit ?? false;
    }

    private static bool CanSubmit(ProjectUpdateWorkflowContext ctx, Dictionary<string, WorkflowStepStatus> steps)
    {
        // Must be in editable state
        if (ctx.ProjectUpdateStateID != (int)ProjectUpdateStateEnum.Created &&
            ctx.ProjectUpdateStateID != (int)ProjectUpdateStateEnum.Returned)
            return false;

        // Must pass all 4 validation rules (matching legacy IsPassingAllValidationRules)
        return IsBasicsComplete(ctx) &&
               IsLocationSimpleComplete(ctx) &&
               IsPriorityLandscapesComplete(ctx) &&
               IsDnrUplandRegionsComplete(ctx);
    }

    private static bool IsReadyToApprove(ProjectUpdateWorkflowContext ctx, Dictionary<string, WorkflowStepStatus> steps)
    {
        // Ready to approve if all required steps are complete (validation rules pass)
        return IsBasicsComplete(ctx) &&
               IsLocationSimpleComplete(ctx) &&
               IsPriorityLandscapesComplete(ctx) &&
               IsDnrUplandRegionsComplete(ctx);
    }

    private static async Task<ProjectUpdateWorkflowContext?> LoadWorkflowContextAsync(WADNRDbContext dbContext, int projectUpdateBatchID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .AsNoTracking()
            .Where(b => b.ProjectUpdateBatchID == projectUpdateBatchID)
            .Select(b => new ProjectUpdateWorkflowContext
            {
                ProjectUpdateBatchID = b.ProjectUpdateBatchID,
                ProjectID = b.ProjectID,
                ProjectName = b.Project.ProjectName,
                ProjectUpdateStateID = b.ProjectUpdateStateID,
                ProjectUpdateStateName = null, // Resolved client-side below
                LastUpdateDate = b.LastUpdateDate,
                LastUpdatedByPersonName = b.LastUpdatePerson.FirstName + " " + b.LastUpdatePerson.LastName,
                ProjectStageID = b.ProjectUpdates.FirstOrDefault() != null ? b.ProjectUpdates.First().ProjectStageID : 0,
                PlannedDate = b.ProjectUpdates.FirstOrDefault() != null ? b.ProjectUpdates.First().PlannedDate : null,
                HasSimpleLocation = b.ProjectUpdates.Any(pu => pu.ProjectLocationPoint != null),
                ProjectLocationNotes = b.ProjectUpdates.Select(pu => pu.ProjectLocationNotes).FirstOrDefault(),
                ProjectLocationSimpleTypeID = b.ProjectUpdates.FirstOrDefault() != null ? b.ProjectUpdates.First().ProjectLocationSimpleTypeID : 1,
                HasDetailedLocations = b.ProjectLocationUpdates.Any(),
                HasPriorityLandscapes = b.ProjectPriorityLandscapeUpdates.Any(),
                NoPriorityLandscapesExplanation = b.NoPriorityLandscapesExplanation,
                HasDnrUplandRegions = b.ProjectRegionUpdates.Any(),
                NoRegionsExplanation = b.NoRegionsExplanation,
                HasCounties = b.ProjectCountyUpdates.Any(),
                NoCountiesExplanation = b.NoCountiesExplanation,
                HasTreatments = b.TreatmentUpdates.Any(),
                HasContacts = b.ProjectPersonUpdates.Any(),
                HasOrganizations = b.ProjectOrganizationUpdates.Any(),
                HasLeadImplementer = b.ProjectOrganizationUpdates.Any(ou => ou.RelationshipType.IsPrimaryContact),
                HasExpectedFunding = b.ProjectFundSourceAllocationRequestUpdates.Any() ||
                    b.ProjectUpdates.Any(pu => pu.EstimatedTotalCost != null),
                HasPhotos = b.ProjectImageUpdates.Any(),
                HasExternalLinks = b.ProjectExternalLinkUpdates.Any(),
                HasDocuments = b.ProjectDocumentUpdates.Any(),
                HasNotes = b.ProjectNoteUpdates.Any(),

                // Reviewer comments
                BasicsComment = b.BasicsComment,
                LocationSimpleComment = b.LocationSimpleComment,
                LocationDetailedComment = b.LocationDetailedComment,
                ExpectedFundingComment = b.ExpectedFundingComment,
                ContactsComment = b.ContactsComment,
                OrganizationsComment = b.OrganizationsComment
            })
            .SingleOrDefaultAsync();

        if (batch == null) return null;

        // Resolve lookup value client-side to avoid EF Core translation issues
        if (ProjectUpdateState.AllLookupDictionary.TryGetValue(batch.ProjectUpdateStateID, out var state))
        {
            batch = batch with { ProjectUpdateStateName = state.ProjectUpdateStateDisplayName };
        }

        // Fetch submitted/returned history separately for cleaner queries
        var histories = await dbContext.ProjectUpdateHistories
            .AsNoTracking()
            .Where(h => h.ProjectUpdateBatchID == projectUpdateBatchID)
            .OrderByDescending(h => h.TransitionDate)
            .Select(h => new
            {
                h.ProjectUpdateStateID,
                h.TransitionDate,
                PersonName = h.UpdatePerson.FirstName + " " + h.UpdatePerson.LastName
            })
            .ToListAsync();

        // Get the most recent Submitted history
        var submittedHistory = histories.FirstOrDefault(h =>
            h.ProjectUpdateStateID == (int)ProjectUpdateStateEnum.Submitted);
        if (submittedHistory != null)
        {
            batch = batch with
            {
                SubmittedByPersonName = submittedHistory.PersonName,
                SubmittedDate = submittedHistory.TransitionDate
            };
        }

        // Get the most recent Returned history
        var returnedHistory = histories.FirstOrDefault(h =>
            h.ProjectUpdateStateID == (int)ProjectUpdateStateEnum.Returned);
        if (returnedHistory != null)
        {
            batch = batch with
            {
                ReturnedByPersonName = returnedHistory.PersonName,
                ReturnedDate = returnedHistory.TransitionDate
            };
        }

        return batch;
    }

    /// <summary>
    /// Determines if a step is currently active (not disabled) based on batch state.
    /// </summary>
    private static bool IsStepActive(ProjectUpdateWorkflowContext ctx, ProjectUpdateWorkflowStep step)
    {
        // All steps are always active for Update workflow (batch already exists)
        return true;
    }

    /// <summary>
    /// Determines if a step is required for submission.
    /// </summary>
    private static bool IsStepRequired(ProjectUpdateWorkflowStep step)
    {
        // Only the 4 steps checked by legacy IsPassingAllValidationRules are required
        return step switch
        {
            ProjectUpdateWorkflowStep.Basics => true,
            ProjectUpdateWorkflowStep.LocationSimple => true,
            ProjectUpdateWorkflowStep.PriorityLandscapes => true,
            ProjectUpdateWorkflowStep.DnrUplandRegions => true,
            // Counties, Contacts, Organizations are optional (legacy didn't gate submission on these)
            _ => false
        };
    }

    /// <summary>
    /// Determines if a step is complete based on update batch data.
    /// </summary>
    private static bool IsStepComplete(ProjectUpdateWorkflowContext ctx, ProjectUpdateWorkflowStep step)
    {
        return step switch
        {
            ProjectUpdateWorkflowStep.Basics => IsBasicsComplete(ctx),
            ProjectUpdateWorkflowStep.LocationSimple => IsLocationSimpleComplete(ctx),
            ProjectUpdateWorkflowStep.LocationDetailed => ctx.HasDetailedLocations,
            ProjectUpdateWorkflowStep.PriorityLandscapes => IsPriorityLandscapesComplete(ctx),
            ProjectUpdateWorkflowStep.DnrUplandRegions => IsDnrUplandRegionsComplete(ctx),
            ProjectUpdateWorkflowStep.Counties => IsCountiesComplete(ctx),
            ProjectUpdateWorkflowStep.Treatments => ctx.HasTreatments,
            ProjectUpdateWorkflowStep.Contacts => ctx.HasContacts,
            ProjectUpdateWorkflowStep.Organizations => IsOrganizationsComplete(ctx),
            ProjectUpdateWorkflowStep.ExpectedFunding => ctx.HasExpectedFunding,
            ProjectUpdateWorkflowStep.Photos => ctx.HasPhotos,
            ProjectUpdateWorkflowStep.ExternalLinks => ctx.HasExternalLinks,
            ProjectUpdateWorkflowStep.DocumentsNotes => ctx.HasDocuments || ctx.HasNotes,
            _ => false
        };
    }

    private static bool IsBasicsComplete(ProjectUpdateWorkflowContext ctx)
    {
        // Required field: ProjectStageID (PlannedDate is NOT required per legacy validation)
        return ctx.ProjectStageID > 0;
    }

    private static bool IsLocationSimpleComplete(ProjectUpdateWorkflowContext ctx)
    {
        // Location point OR notes (legacy allowed notes as alternative to a map point)
        return ctx.HasSimpleLocation || !string.IsNullOrWhiteSpace(ctx.ProjectLocationNotes);
    }

    private static bool IsPriorityLandscapesComplete(ProjectUpdateWorkflowContext ctx)
    {
        // Either has priority landscapes OR has an explanation for why none
        return ctx.HasPriorityLandscapes || !string.IsNullOrWhiteSpace(ctx.NoPriorityLandscapesExplanation);
    }

    private static bool IsDnrUplandRegionsComplete(ProjectUpdateWorkflowContext ctx)
    {
        // Either has regions OR has an explanation for why none
        return ctx.HasDnrUplandRegions || !string.IsNullOrWhiteSpace(ctx.NoRegionsExplanation);
    }

    private static bool IsCountiesComplete(ProjectUpdateWorkflowContext ctx)
    {
        // Either has counties OR has an explanation for why none
        return ctx.HasCounties || !string.IsNullOrWhiteSpace(ctx.NoCountiesExplanation);
    }

    private static bool IsOrganizationsComplete(ProjectUpdateWorkflowContext ctx)
    {
        // Must have at least one organization with lead implementer designation
        return ctx.HasLeadImplementer;
    }

    /// <summary>
    /// Builds the reviewer comments dictionary mapping step keys to comment text.
    /// Geographic area steps (PriorityLandscapes, DnrUplandRegions, Counties) share LocationSimpleComment.
    /// </summary>
    private static Dictionary<string, string?> BuildReviewerComments(ProjectUpdateWorkflowContext ctx)
    {
        return new Dictionary<string, string?>
        {
            ["Basics"] = ctx.BasicsComment,
            ["LocationSimple"] = ctx.LocationSimpleComment,
            ["LocationDetailed"] = ctx.LocationDetailedComment,
            ["PriorityLandscapes"] = ctx.LocationSimpleComment,
            ["DnrUplandRegions"] = ctx.LocationSimpleComment,
            ["Counties"] = ctx.LocationSimpleComment,
            ["ExpectedFunding"] = ctx.ExpectedFundingComment,
            ["Contacts"] = ctx.ContactsComment,
            ["Organizations"] = ctx.OrganizationsComment
        };
    }

    /// <summary>
    /// Computes which steps have changes compared to the approved project.
    /// Uses lightweight comparison for performance (not full HTML diff).
    /// </summary>
    private static async Task<Dictionary<string, bool>> GetStepChangesAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        int projectID)
    {
        var result = new Dictionary<string, bool>();

        // Load batch and project scalars only — child collections are loaded separately
        // to avoid massive Cartesian product joins (especially with geometry columns).
        var batch = await dbContext.ProjectUpdateBatches
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        var project = await dbContext.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProjectID == projectID);

        if (batch == null || project == null) return result;

        var projectUpdate = await dbContext.Set<ProjectUpdate>()
            .AsNoTracking()
            .FirstOrDefaultAsync(pu => pu.ProjectUpdateBatchID == projectUpdateBatchID);

        // Load each collection pair separately for comparison
        var projectPrograms = await dbContext.ProjectPrograms.AsNoTracking()
            .Where(x => x.ProjectID == projectID).ToListAsync();
        var updatePrograms = await dbContext.ProjectUpdatePrograms.AsNoTracking()
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).ToListAsync();

        // Basics: compare basic fields and programs
        result["Basics"] = HasBasicsChanges(project, projectUpdate, projectPrograms, updatePrograms);

        // LocationSimple: compare point location
        result["LocationSimple"] = HasLocationSimpleChanges(project, projectUpdate);

        // LocationDetailed: compare location counts and names
        var projectLocations = await dbContext.ProjectLocations.AsNoTracking()
            .Where(x => x.ProjectID == projectID).ToListAsync();
        var updateLocations = await dbContext.ProjectLocationUpdates.AsNoTracking()
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).ToListAsync();
        result["LocationDetailed"] = HasLocationDetailedChanges(projectLocations, updateLocations);

        // PriorityLandscapes
        var projectPriorityLandscapeIDs = await dbContext.ProjectPriorityLandscapes.AsNoTracking()
            .Where(x => x.ProjectID == projectID).Select(x => x.PriorityLandscapeID).ToListAsync();
        var updatePriorityLandscapeIDs = await dbContext.ProjectPriorityLandscapeUpdates.AsNoTracking()
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).Select(x => x.PriorityLandscapeID).ToListAsync();
        result["PriorityLandscapes"] = HasGeographicChanges(
            projectPriorityLandscapeIDs.ToHashSet(),
            updatePriorityLandscapeIDs.ToHashSet(),
            project.NoPriorityLandscapesExplanation,
            batch.NoPriorityLandscapesExplanation);

        // DnrUplandRegions
        var projectRegionIDs = await dbContext.ProjectRegions.AsNoTracking()
            .Where(x => x.ProjectID == projectID).Select(x => x.DNRUplandRegionID).ToListAsync();
        var updateRegionIDs = await dbContext.ProjectRegionUpdates.AsNoTracking()
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).Select(x => x.DNRUplandRegionID).ToListAsync();
        result["DnrUplandRegions"] = HasGeographicChanges(
            projectRegionIDs.ToHashSet(),
            updateRegionIDs.ToHashSet(),
            project.NoRegionsExplanation,
            batch.NoRegionsExplanation);

        // Counties
        var projectCountyIDs = await dbContext.ProjectCounties.AsNoTracking()
            .Where(x => x.ProjectID == projectID).Select(x => x.CountyID).ToListAsync();
        var updateCountyIDs = await dbContext.ProjectCountyUpdates.AsNoTracking()
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).Select(x => x.CountyID).ToListAsync();
        result["Counties"] = HasGeographicChanges(
            projectCountyIDs.ToHashSet(),
            updateCountyIDs.ToHashSet(),
            project.NoCountiesExplanation,
            batch.NoCountiesExplanation);

        // Treatments - compare by count for simplicity
        var projectTreatmentCount = await dbContext.Treatments
            .CountAsync(t => t.ProjectLocation.ProjectID == projectID);
        var updateTreatmentCount = await dbContext.TreatmentUpdates
            .CountAsync(t => t.ProjectUpdateBatchID == projectUpdateBatchID);
        result["Treatments"] = projectTreatmentCount != updateTreatmentCount;

        // Contacts
        var projectPeople = await dbContext.ProjectPeople.AsNoTracking()
            .Where(x => x.ProjectID == projectID).ToListAsync();
        var updatePeople = await dbContext.ProjectPersonUpdates.AsNoTracking()
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).ToListAsync();
        result["Contacts"] = HasContactsChanges(projectPeople, updatePeople);

        // Organizations
        var projectOrganizations = await dbContext.ProjectOrganizations.AsNoTracking()
            .Where(x => x.ProjectID == projectID).ToListAsync();
        var updateOrganizations = await dbContext.ProjectOrganizationUpdates.AsNoTracking()
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).ToListAsync();
        result["Organizations"] = HasOrganizationsChanges(projectOrganizations, updateOrganizations);

        // ExpectedFunding
        var projectFundingSources = await dbContext.ProjectFundingSources.AsNoTracking()
            .Where(x => x.ProjectID == projectID).ToListAsync();
        var updateFundingSources = await dbContext.ProjectFundingSourceUpdates.AsNoTracking()
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).ToListAsync();
        var projectAllocations = await dbContext.ProjectFundSourceAllocationRequests.AsNoTracking()
            .Where(x => x.ProjectID == projectID).ToListAsync();
        var updateAllocations = await dbContext.ProjectFundSourceAllocationRequestUpdates.AsNoTracking()
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).ToListAsync();
        result["ExpectedFunding"] = HasExpectedFundingChanges(project, projectUpdate,
            projectFundingSources, updateFundingSources, projectAllocations, updateAllocations);

        // Photos
        var projectImages = await dbContext.ProjectImages.AsNoTracking()
            .Where(x => x.ProjectID == projectID).ToListAsync();
        var updateImages = await dbContext.ProjectImageUpdates.AsNoTracking()
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).ToListAsync();
        result["Photos"] = HasPhotosChanges(projectImages, updateImages);

        // ExternalLinks
        var projectExternalLinks = await dbContext.ProjectExternalLinks.AsNoTracking()
            .Where(x => x.ProjectID == projectID).ToListAsync();
        var updateExternalLinks = await dbContext.ProjectExternalLinkUpdates.AsNoTracking()
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).ToListAsync();
        result["ExternalLinks"] = HasExternalLinksChanges(projectExternalLinks, updateExternalLinks);

        // DocumentsNotes
        var projectDocuments = await dbContext.ProjectDocuments.AsNoTracking()
            .Where(x => x.ProjectID == projectID).ToListAsync();
        var projectNotes = await dbContext.ProjectNotes.AsNoTracking()
            .Where(x => x.ProjectID == projectID).ToListAsync();
        var updateDocuments = await dbContext.ProjectDocumentUpdates.AsNoTracking()
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).ToListAsync();
        var updateNotes = await dbContext.ProjectNoteUpdates.AsNoTracking()
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).ToListAsync();
        result["DocumentsNotes"] = HasDocumentsNotesChanges(projectDocuments, projectNotes, updateDocuments, updateNotes);

        return result;
    }

    private static bool HasBasicsChanges(Project project, ProjectUpdate? update, List<ProjectProgram> projectPrograms, List<ProjectUpdateProgram> updatePrograms)
    {
        if (update == null) return false;

        // Check basic fields
        if (project.ProjectDescription != update.ProjectDescription) return true;
        if (project.ProjectStageID != update.ProjectStageID) return true;
        if (project.PlannedDate != update.PlannedDate) return true;
        if (project.CompletionDate != update.CompletionDate) return true;
        if (project.ExpirationDate != update.ExpirationDate) return true;
        if (project.FocusAreaID != update.FocusAreaID) return true;
        if (project.PercentageMatch != update.PercentageMatch) return true;

        // Check programs
        var projectProgramIDs = projectPrograms.Select(p => p.ProgramID).OrderBy(x => x).ToList();
        var updateProgramIDs = updatePrograms.Select(p => p.ProgramID).OrderBy(x => x).ToList();
        return !projectProgramIDs.SequenceEqual(updateProgramIDs);
    }

    private static bool HasLocationSimpleChanges(Project project, ProjectUpdate? update)
    {
        if (update == null) return false;

        // Compare point locations
        var projLat = project.ProjectLocationPoint?.Coordinate.Y;
        var projLon = project.ProjectLocationPoint?.Coordinate.X;
        var updateLat = update.ProjectLocationPoint?.Coordinate.Y;
        var updateLon = update.ProjectLocationPoint?.Coordinate.X;

        if (projLat != updateLat || projLon != updateLon) return true;
        if (project.ProjectLocationSimpleTypeID != update.ProjectLocationSimpleTypeID) return true;
        if (project.ProjectLocationNotes != update.ProjectLocationNotes) return true;

        return false;
    }

    private static bool HasLocationDetailedChanges(List<ProjectLocation> projectLocs, List<ProjectLocationUpdate> updateLocs)
    {
        if (projectLocs.Count != updateLocs.Count) return true;

        var projByName = projectLocs.OrderBy(l => l.ProjectLocationName).ToList();
        var updateByName = updateLocs.OrderBy(l => l.ProjectLocationUpdateName).ToList();

        if (!projByName.Select(l => l.ProjectLocationName).SequenceEqual(updateByName.Select(l => l.ProjectLocationUpdateName)))
            return true;

        for (int i = 0; i < projByName.Count; i++)
        {
            var proj = projByName[i];
            var upd = updateByName[i];

            if (proj.ProjectLocationTypeID != upd.ProjectLocationTypeID) return true;
            if (proj.ProjectLocationNotes != upd.ProjectLocationUpdateNotes) return true;

            var projGeom = proj.ProjectLocationGeometry;
            var updGeom = upd.ProjectLocationUpdateGeometry;
            if (projGeom == null && updGeom != null) return true;
            if (projGeom != null && updGeom == null) return true;
            if (projGeom != null && updGeom != null && !projGeom.EqualsTopologically(updGeom)) return true;
        }

        return false;
    }

    private static bool HasGeographicChanges(HashSet<int> projectIDs, HashSet<int> updateIDs, string? projectExplanation, string? updateExplanation)
    {
        if (!projectIDs.SetEquals(updateIDs)) return true;
        return projectExplanation != updateExplanation;
    }

    private static bool HasContactsChanges(List<ProjectPerson> projectContacts, List<ProjectPersonUpdate> updateContacts)
    {
        if (projectContacts.Count != updateContacts.Count) return true;

        var projPairs = projectContacts.Select(c => (c.PersonID, c.ProjectPersonRelationshipTypeID)).OrderBy(x => x).ToList();
        var updatePairs = updateContacts.Select(c => (c.PersonID, c.ProjectPersonRelationshipTypeID)).OrderBy(x => x).ToList();
        return !projPairs.SequenceEqual(updatePairs);
    }

    private static bool HasOrganizationsChanges(List<ProjectOrganization> projectOrgs, List<ProjectOrganizationUpdate> updateOrgs)
    {
        if (projectOrgs.Count != updateOrgs.Count) return true;

        var projPairs = projectOrgs.Select(o => (o.OrganizationID, o.RelationshipTypeID)).OrderBy(x => x).ToList();
        var updatePairs = updateOrgs.Select(o => (o.OrganizationID, o.RelationshipTypeID)).OrderBy(x => x).ToList();
        return !projPairs.SequenceEqual(updatePairs);
    }

    private static bool HasExpectedFundingChanges(
        Project project, ProjectUpdate? update,
        List<ProjectFundingSource> projectFundingSources, List<ProjectFundingSourceUpdate> updateFundingSources,
        List<ProjectFundSourceAllocationRequest> projectAllocations, List<ProjectFundSourceAllocationRequestUpdate> updateAllocations)
    {
        if (update != null)
        {
            if (project.EstimatedTotalCost != update.EstimatedTotalCost) return true;
            if (project.ProjectFundingSourceNotes != update.ProjectFundingSourceNotes) return true;
        }

        var projFundingIDs = projectFundingSources.Select(f => f.FundingSourceID).OrderBy(x => x).ToList();
        var updateFundingIDs = updateFundingSources.Select(f => f.FundingSourceID).OrderBy(x => x).ToList();
        if (!projFundingIDs.SequenceEqual(updateFundingIDs)) return true;

        if (projectAllocations.Count != updateAllocations.Count) return true;

        var projAllocs = projectAllocations
            .OrderBy(a => a.ProjectFundSourceAllocationRequestID)
            .Select(a => (a.TotalAmount, a.MatchAmount, a.PayAmount))
            .ToList();
        var updateAllocs = updateAllocations
            .OrderBy(a => a.ProjectFundSourceAllocationRequestUpdateID)
            .Select(a => (a.TotalAmount, a.MatchAmount, a.PayAmount))
            .ToList();
        if (!projAllocs.SequenceEqual(updateAllocs)) return true;

        return false;
    }

    private static bool HasPhotosChanges(List<ProjectImage> projectImages, List<ProjectImageUpdate> updateImages)
    {
        if (projectImages.Count != updateImages.Count) return true;

        var projPhotos = projectImages
            .Select(i => (i.FileResourceID, i.IsKeyPhoto, i.Caption, i.Credit, i.ExcludeFromFactSheet, i.ProjectImageTimingID))
            .OrderBy(x => x.FileResourceID).ToList();
        var updatePhotos = updateImages
            .Select(i => (i.FileResourceID ?? 0, i.IsKeyPhoto, i.Caption, i.Credit, i.ExcludeFromFactSheet, i.ProjectImageTimingID))
            .OrderBy(x => x.Item1).ToList();
        return !projPhotos.SequenceEqual(updatePhotos);
    }

    private static bool HasExternalLinksChanges(List<ProjectExternalLink> projectLinks, List<ProjectExternalLinkUpdate> updateLinks)
    {
        if (projectLinks.Count != updateLinks.Count) return true;

        var projLinks = projectLinks.Select(l => (l.ExternalLinkLabel, l.ExternalLinkUrl)).OrderBy(x => x.ExternalLinkLabel).ToList();
        var updateLinkData = updateLinks.Select(l => (l.ExternalLinkLabel, l.ExternalLinkUrl)).OrderBy(x => x.ExternalLinkLabel).ToList();
        return !projLinks.SequenceEqual(updateLinkData);
    }

    private static bool HasDocumentsNotesChanges(
        List<ProjectDocument> projectDocs,
        List<ProjectNote> projectNotes,
        List<ProjectDocumentUpdate> updateDocs,
        List<ProjectNoteUpdate> updateNotes)
    {
        if (projectDocs.Count != updateDocs.Count) return true;
        if (projectNotes.Count != updateNotes.Count) return true;

        var projDocTitles = projectDocs.Select(d => d.DisplayName).OrderBy(x => x).ToList();
        var updateDocTitles = updateDocs.Select(d => d.DisplayName).OrderBy(x => x).ToList();
        if (!projDocTitles.SequenceEqual(updateDocTitles)) return true;

        var projNoteTexts = projectNotes.Select(n => n.Note).OrderBy(x => x).ToList();
        var updateNoteTexts = updateNotes.Select(n => n.Note).OrderBy(x => x).ToList();
        return !projNoteTexts.SequenceEqual(updateNoteTexts);
    }
}
