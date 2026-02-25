using Microsoft.EntityFrameworkCore;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Workflows;

/// <summary>
/// Workflow progress computation for the ProjectCreate wizard.
/// Determines step completion states and whether a project can be submitted for approval.
/// </summary>
public static class ProjectCreateWorkflowProgress
{
    /// <summary>
    /// All steps in the ProjectCreate wizard, in order.
    /// </summary>
    public enum ProjectCreateWorkflowStep
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
        Classifications = 11,
        Photos = 12,
        DocumentsNotes = 13
    }

    /// <summary>
    /// Internal context loaded once for computing all step states.
    /// </summary>
    private sealed class ProjectCreateWorkflowContext
    {
        public int ProjectID { get; init; }
        public string ProjectName { get; init; } = string.Empty;
        public int ProjectApprovalStatusID { get; init; }
        public string ProjectApprovalStatusName { get; init; } = string.Empty;
        public string? CreatedByPersonName { get; init; }
        public string? CreatedByOrganizationName { get; init; }
        public DateTime? CreateDate { get; init; }
        public int ProjectTypeID { get; init; }
        public int? ProjectStageID { get; init; }
        public int? FocusAreaID { get; init; }
        public DateTime? PlannedDate { get; init; }
        public bool HasSimpleLocation { get; init; }
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
        public bool HasClassifications { get; init; }
        public bool HasPhotos { get; init; }
        public bool HasDocuments { get; init; }
        public bool HasNotes { get; init; }
        public int ProgramCount { get; init; }
    }

    /// <summary>
    /// Gets the workflow progress for a project, including step completion states and submit eligibility.
    /// </summary>
    public static async Task<CreateWorkflowProgressResponse?> GetProgressAsync(WADNRDbContext dbContext, int projectID)
    {
        return await GetProgressForUserAsync(dbContext, projectID, null);
    }

    /// <summary>
    /// Gets the workflow progress for a project, including user-specific permission flags.
    /// </summary>
    public static async Task<CreateWorkflowProgressResponse?> GetProgressForUserAsync(
        WADNRDbContext dbContext,
        int projectID,
        PersonDetail? callingUser)
    {
        var ctx = await LoadWorkflowContextAsync(dbContext, projectID);
        if (ctx == null) return null;

        var steps = new Dictionary<ProjectCreateWorkflowStep, WorkflowStepStatus>();
        foreach (var step in Enum.GetValues<ProjectCreateWorkflowStep>())
        {
            steps[step] = new WorkflowStepStatus
            {
                IsComplete = IsStepComplete(ctx, step),
                IsDisabled = !IsStepActive(ctx, step),
                IsRequired = IsStepRequired(step)
            };
        }

        var dto = new CreateWorkflowProgressResponse
        {
            ProjectID = ctx.ProjectID,
            ProjectName = ctx.ProjectName,
            ProjectApprovalStatusID = ctx.ProjectApprovalStatusID,
            ProjectApprovalStatusName = ctx.ProjectApprovalStatusName,
            CanSubmit = CanSubmit(ctx, steps),
            CreatedByPersonName = ctx.CreatedByPersonName,
            CreatedByOrganizationName = ctx.CreatedByOrganizationName,
            CreateDate = ctx.CreateDate,
            Steps = steps
        };

        // Populate user permission flags
        PopulateUserPermissionFlags(dto, ctx, callingUser, dbContext);

        return dto;
    }

    /// <summary>
    /// Populates user permission flags based on calling user's role and project state.
    /// </summary>
    private static void PopulateUserPermissionFlags(
        CreateWorkflowProgressResponse dto,
        ProjectCreateWorkflowContext ctx,
        PersonDetail? callingUser,
        WADNRDbContext dbContext)
    {
        // Default all to false for anonymous/unassigned users
        if (callingUser == null || callingUser.IsAnonymousOrUnassigned())
        {
            dto.CanApprove = false;
            dto.CanReject = false;
            dto.CanReturn = false;
            dto.CanWithdraw = false;
            dto.CanEdit = false;
            return;
        }

        var isPendingApproval = ctx.ProjectApprovalStatusID == (int)ProjectApprovalStatusEnum.PendingApproval;
        var isDraftOrReturned = ctx.ProjectApprovalStatusID == (int)ProjectApprovalStatusEnum.Draft ||
                                ctx.ProjectApprovalStatusID == (int)ProjectApprovalStatusEnum.Returned;

        // Approve/Reject/Return: only available to approvers (Admin, EsaAdmin, ProjectSteward, CanEditProgram)
        // and only when project is PendingApproval
        var canApproveProjects = callingUser.HasElevatedProjectAccess() || callingUser.HasCanEditProgramRole();
        dto.CanApprove = canApproveProjects && isPendingApproval;
        dto.CanReject = canApproveProjects && isPendingApproval;
        dto.CanReturn = canApproveProjects && isPendingApproval;

        // Withdraw: available to editor when PendingApproval
        dto.CanWithdraw = isPendingApproval;

        // Edit: based on role and project organizations
        if (callingUser.HasElevatedProjectAccess())
        {
            dto.CanEdit = true;
        }
        else
        {
            // Need to check if user's org is associated with the project
            var userOrgID = callingUser.OrganizationID;
            if (userOrgID.HasValue)
            {
                var projectOrgIds = dbContext.ProjectOrganizations
                    .AsNoTracking()
                    .Where(po => po.ProjectID == ctx.ProjectID)
                    .Select(po => po.OrganizationID)
                    .ToList();
                dto.CanEdit = projectOrgIds.Contains(userOrgID.Value);
            }
            else
            {
                dto.CanEdit = false;
            }
        }
    }

    /// <summary>
    /// Checks if all required steps are complete and the project can be submitted for approval.
    /// </summary>
    public static async Task<bool> CanSubmitAsync(WADNRDbContext dbContext, int projectID)
    {
        var progress = await GetProgressAsync(dbContext, projectID);
        return progress?.CanSubmit ?? false;
    }

    private static bool CanSubmit(ProjectCreateWorkflowContext ctx, Dictionary<ProjectCreateWorkflowStep, WorkflowStepStatus> steps)
    {
        // Project must be in Draft or Returned status to submit
        if (ctx.ProjectApprovalStatusID != (int)ProjectApprovalStatusEnum.Draft &&
            ctx.ProjectApprovalStatusID != (int)ProjectApprovalStatusEnum.Returned)
        {
            return false;
        }

        // All required steps must be complete
        return steps
            .Where(kvp => kvp.Value.IsRequired)
            .All(kvp => kvp.Value.IsComplete);
    }

    private static async Task<ProjectCreateWorkflowContext?> LoadWorkflowContextAsync(WADNRDbContext dbContext, int projectID)
    {
        var project = await dbContext.Projects
            .AsNoTracking()
            .Where(p => p.ProjectID == projectID)
            .Select(p => new ProjectCreateWorkflowContext
            {
                ProjectID = p.ProjectID,
                ProjectName = p.ProjectName,
                ProjectApprovalStatusID = p.ProjectApprovalStatusID,
                ProjectApprovalStatusName = p.ProjectApprovalStatus.ProjectApprovalStatusDisplayName,
                CreatedByPersonName = p.ProposingPerson != null ? p.ProposingPerson.FirstName + " " + p.ProposingPerson.LastName : null,
                CreatedByOrganizationName = p.ProposingPerson != null && p.ProposingPerson.Organization != null ? p.ProposingPerson.Organization.OrganizationName : null,
                CreateDate = p.ProposingDate,
                ProjectTypeID = p.ProjectTypeID,
                ProjectStageID = p.ProjectStageID,
                FocusAreaID = p.FocusAreaID,
                PlannedDate = p.PlannedDate,
                HasSimpleLocation = p.ProjectLocationPoint != null,
                ProjectLocationSimpleTypeID = p.ProjectLocationSimpleTypeID,
                HasDetailedLocations = p.ProjectLocations.Any(),
                HasPriorityLandscapes = p.ProjectPriorityLandscapes.Any(),
                NoPriorityLandscapesExplanation = p.NoPriorityLandscapesExplanation,
                HasDnrUplandRegions = p.ProjectRegions.Any(),
                NoRegionsExplanation = p.NoRegionsExplanation,
                HasCounties = p.ProjectCounties.Any(),
                NoCountiesExplanation = p.NoCountiesExplanation,
                HasTreatments = p.Treatments.Any(),
                HasContacts = p.ProjectPeople.Any(),
                HasOrganizations = p.ProjectOrganizations.Any(),
                HasLeadImplementer = p.ProjectOrganizations.Any(po => po.RelationshipType.IsPrimaryContact),
                HasExpectedFunding = p.ProjectFundSourceAllocationRequests.Any() || p.EstimatedTotalCost != null,
                HasClassifications = p.ProjectClassifications.Any(),
                HasPhotos = p.ProjectImages.Any(),
                HasDocuments = p.ProjectDocuments.Any(),
                HasNotes = p.ProjectNotes.Any(),
                ProgramCount = p.ProjectPrograms.Count
            })
            .SingleOrDefaultAsync();

        return project;
    }

    /// <summary>
    /// Determines if a step is currently active (not disabled) based on project state.
    /// </summary>
    private static bool IsStepActive(ProjectCreateWorkflowContext ctx, ProjectCreateWorkflowStep step)
    {
        // Basics step is always active (even for new projects)
        if (step == ProjectCreateWorkflowStep.Basics)
            return true;

        // All other steps require the project to exist (ProjectID > 0)
        // which means Basics has been completed
        return ctx.ProjectID > 0;
    }

    /// <summary>
    /// Determines if a step is required for submission.
    /// </summary>
    private static bool IsStepRequired(ProjectCreateWorkflowStep step)
    {
        return step switch
        {
            ProjectCreateWorkflowStep.Basics => true,
            ProjectCreateWorkflowStep.LocationSimple => true,
            // All other steps are optional
            _ => false
        };
    }

    /// <summary>
    /// Determines if a step is complete based on project data.
    /// </summary>
    private static bool IsStepComplete(ProjectCreateWorkflowContext ctx, ProjectCreateWorkflowStep step)
    {
        return step switch
        {
            ProjectCreateWorkflowStep.Basics => IsBasicsComplete(ctx),
            ProjectCreateWorkflowStep.LocationSimple => IsLocationSimpleComplete(ctx),
            ProjectCreateWorkflowStep.LocationDetailed => ctx.HasDetailedLocations,
            ProjectCreateWorkflowStep.PriorityLandscapes => IsPriorityLandscapesComplete(ctx),
            ProjectCreateWorkflowStep.DnrUplandRegions => IsDnrUplandRegionsComplete(ctx),
            ProjectCreateWorkflowStep.Counties => IsCountiesComplete(ctx),
            ProjectCreateWorkflowStep.Treatments => ctx.HasTreatments,
            ProjectCreateWorkflowStep.Contacts => ctx.HasContacts,
            ProjectCreateWorkflowStep.Organizations => IsOrganizationsComplete(ctx),
            ProjectCreateWorkflowStep.ExpectedFunding => ctx.HasExpectedFunding,
            ProjectCreateWorkflowStep.Classifications => ctx.HasClassifications,
            ProjectCreateWorkflowStep.Photos => ctx.HasPhotos,
            ProjectCreateWorkflowStep.DocumentsNotes => ctx.HasDocuments || ctx.HasNotes,
            _ => false
        };
    }

    private static bool IsBasicsComplete(ProjectCreateWorkflowContext ctx)
    {
        // Required fields: ProjectTypeID, ProjectName, ProjectStageID, PlannedDate
        return ctx.ProjectTypeID > 0 &&
               !string.IsNullOrWhiteSpace(ctx.ProjectName) &&
               ctx.ProjectStageID.HasValue && ctx.ProjectStageID.Value > 0 &&
               ctx.PlannedDate.HasValue;
    }

    private static bool IsLocationSimpleComplete(ProjectCreateWorkflowContext ctx)
    {
        // Must have a location point set
        return ctx.HasSimpleLocation;
    }

    private static bool IsPriorityLandscapesComplete(ProjectCreateWorkflowContext ctx)
    {
        // Either has priority landscapes OR has an explanation for why none
        return ctx.HasPriorityLandscapes || !string.IsNullOrWhiteSpace(ctx.NoPriorityLandscapesExplanation);
    }

    private static bool IsDnrUplandRegionsComplete(ProjectCreateWorkflowContext ctx)
    {
        // Either has regions OR has an explanation for why none
        return ctx.HasDnrUplandRegions || !string.IsNullOrWhiteSpace(ctx.NoRegionsExplanation);
    }

    private static bool IsCountiesComplete(ProjectCreateWorkflowContext ctx)
    {
        // Either has counties OR has an explanation for why none
        return ctx.HasCounties || !string.IsNullOrWhiteSpace(ctx.NoCountiesExplanation);
    }

    private static bool IsOrganizationsComplete(ProjectCreateWorkflowContext ctx)
    {
        // Must have at least one organization with lead implementer designation
        return ctx.HasLeadImplementer;
    }
}

/// <summary>
/// Response for Create workflow progress returned by the API.
/// </summary>
public class CreateWorkflowProgressResponse
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int ProjectApprovalStatusID { get; set; }
    public string ProjectApprovalStatusName { get; set; } = string.Empty;
    public bool CanSubmit { get; set; }
    public string? CreatedByPersonName { get; set; }
    public string? CreatedByOrganizationName { get; set; }
    public DateTime? CreateDate { get; set; }
    public Dictionary<ProjectCreateWorkflowProgress.ProjectCreateWorkflowStep, WorkflowStepStatus> Steps { get; set; } = new();

    // User permission flags (populated based on calling user's role and project state)
    public bool CanApprove { get; set; }
    public bool CanReject { get; set; }
    public bool CanReturn { get; set; }
    public bool CanWithdraw { get; set; }
    public bool CanEdit { get; set; }
}
