using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using System.Linq;
using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class Projects
{
    // Shared EF-usable definition and constant for active projects
    public const int ApprovedStatusId = (int)ProjectApprovalStatusEnum.Approved;

    public static readonly Expression<Func<Project, bool>> IsActiveProjectExpr =
        p => p.ProjectApprovalStatusID == ApprovedStatusId && !p.ProjectType.LimitVisibilityToAdmin;

    // Reusable projection for the project's lead implementer organization (primary contact org)
    public static readonly Expression<Func<Project, OrganizationLookupItem?>> LeadImplementerOrganizationExpr = p =>
        p.ProjectOrganizations
            .Where(po => po.RelationshipType.IsPrimaryContact)
            .Select(po => new OrganizationLookupItem
            {
                OrganizationID = po.Organization.OrganizationID,
                OrganizationName = po.Organization.DisplayName
            })
            .SingleOrDefault();

    public static async Task<List<ProjectCountyDetailGridRow>> ListAsCountyDetailGridRowAsync(WADNRDbContext dbContext, int countyID)
    {
        return await dbContext.Projects
            .Where(p => p.ProjectCounties.Any(pc => pc.CountyID == countyID))
            .AsNoTracking()
            .Where(IsActiveProjectExpr)
            .Select(ProjectProjections.AsProjectCountyDetailGridRow)
            .ToListAsync();
    }

    public static async Task<List<ProjectProjectTypeDetailGridRow>> ListAsProjectTypeDetailGridRowAsync(WADNRDbContext dbContext, int projectTypeID)
    {
        var rows = await dbContext.Projects
            .Where(p => p.ProjectTypeID == projectTypeID)
            .AsNoTracking()
            .Where(IsActiveProjectExpr)
            .OrderBy(x => x.ProjectName)
            .Select(ProjectProjections.AsProjectProjectTypeDetailGridRow)
            .ToListAsync();

        var totalsByProjectId = await GetTotalFundingByProjectAsync(dbContext);

        foreach (var r in rows)
        {
            r.TotalAmount = totalsByProjectId.TryGetValue(r.ProjectID, out var total) ? total : null;
        }

        return rows;
    }

    public static async Task<List<ProjectDNRUplandRegionDetailGridRow>> ListAsDNRUplandDetailGridRowAsync(WADNRDbContext dbContext, int dnrUplandRegionID)
    {

        var rows = await dbContext.Projects
            .Where(p => p.ProjectRegions.Any(x => x.DNRUplandRegionID == dnrUplandRegionID))
            .AsNoTracking()
            .Where(IsActiveProjectExpr)
            .OrderBy(x => x.ProjectName)
            .Select(ProjectProjections.AsDnrUplandRegionDetailGridRow)
            .ToListAsync();

        var totalsByProjectId = await GetTotalTreatedAcresByProjectAsync(dbContext);

        foreach (var r in rows)
        {
            r.TotalTreatedAcres = totalsByProjectId.TryGetValue(r.ProjectID, out var total) ? total : 0m;
        }

        return rows;
    }

    public static async Task<ProjectDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int projectID)
    {
        var entity = await dbContext.Projects
            .AsNoTracking()
            .Where(IsActiveProjectExpr)
            .Where(x => x.ProjectID == projectID)
            .Select(ProjectProjections.AsDetail)
            .SingleOrDefaultAsync();

        if (entity == null) return null;

        // Populate FundingSources from the static lookup table
        var fundingSourceIds = await dbContext.ProjectFundingSources
            .AsNoTracking()
            .Where(pfs => pfs.ProjectID == projectID)
            .Select(pfs => pfs.FundingSourceID)
            .ToListAsync();

        entity.FundingSources = fundingSourceIds
            .Select(id => FundingSource.AllLookupDictionary.TryGetValue(id, out var fs) ? fs.FundingSourceDisplayName : null)
            .Where(name => name != null)
            .Cast<string>()
            .OrderBy(name => name)
            .ToList();

        // Populate bounding box using fallback chain
        entity.DefaultBoundingBox = await GetProjectBoundingBoxAsync(dbContext, projectID);

        return entity;
    }

    public static async Task<ProjectDetail?> CreateAsync(WADNRDbContext dbContext, ProjectUpsertRequest dto, int callingPersonID)
    {
        var entity = new Project
        {
            ProjectTypeID = dto.ProjectTypeID,
            ProjectStageID = dto.ProjectStageID,
            ProjectName = dto.ProjectName,
            ProjectDescription = dto.ProjectDescription,
            CompletionDate = dto.CompletionDate,
            EstimatedTotalCost = dto.EstimatedTotalCost,
            IsFeatured = false,
            ProjectLocationSimpleTypeID = dto.ProjectLocationSimpleTypeID,
            ProjectApprovalStatusID = dto.ProjectApprovalStatusID,
            ProposingPersonID = dto.ProposingPersonID,
            ProposingDate = dto.ProposingDate,
            SubmissionDate = dto.SubmissionDate,
            ApprovalDate = dto.ApprovalDate,
            FocusAreaID = dto.FocusAreaID,
            ExpirationDate = dto.ExpirationDate,
            FhtProjectNumber = dto.FhtProjectNumber
        };
        dbContext.Projects.Add(entity);
        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.ProjectID);
    }

    public static async Task<ProjectDetail?> UpdateAsync(WADNRDbContext dbContext, int projectID, ProjectUpsertRequest dto, int callingPersonID)
    {
        var entity = await dbContext.Projects
            .FirstAsync(x => x.ProjectID == projectID);

        entity.ProjectTypeID = dto.ProjectTypeID;
        entity.ProjectStageID = dto.ProjectStageID;
        entity.ProjectName = dto.ProjectName;
        entity.ProjectDescription = dto.ProjectDescription;
        entity.CompletionDate = dto.CompletionDate;
        entity.EstimatedTotalCost = dto.EstimatedTotalCost;
        entity.ProjectLocationSimpleTypeID = dto.ProjectLocationSimpleTypeID;
        entity.ProjectApprovalStatusID = dto.ProjectApprovalStatusID;
        entity.ProposingPersonID = dto.ProposingPersonID;
        entity.ProposingDate = dto.ProposingDate;
        entity.SubmissionDate = dto.SubmissionDate;
        entity.ApprovalDate = dto.ApprovalDate;
        entity.FocusAreaID = dto.FocusAreaID;
        entity.ExpirationDate = dto.ExpirationDate;
        entity.FhtProjectNumber = dto.FhtProjectNumber;

        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.ProjectID);
    }

    public static async Task<bool> DeleteAsync(WADNRDbContext dbContext, int projectID)
    {
        var deletedCount = await dbContext.Projects
            .Where(x => x.ProjectID == projectID)
            .ExecuteDeleteAsync();
        return deletedCount > 0;
    }

    public static async Task<List<ProjectGridRow>> ListAsGridRowAsync(
        IQueryable<Project> projectsQuery,
        WADNRDbContext dbContext)
    {
        var projects = await projectsQuery
            .AsNoTracking()
            .Where(IsActiveProjectExpr)
            .OrderBy(x => x.ProjectName)
            .Select(ProjectProjections.AsGridRow)
            .ToListAsync();

        var totalsByProjectId = await GetTotalTreatedAcresByProjectAsync(dbContext);

        foreach (var p in projects)
        {
            p.TotalTreatedAcres = totalsByProjectId.TryGetValue(p.ProjectID, out var total) ? total : 0m;
        }

        return projects;
    }

    // Convenience overload for all projects
    public static Task<List<ProjectGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
        => ListAsGridRowAsync(dbContext.Projects, dbContext);

    public static async Task<List<ProjectSimpleTree>> ListWithNoSimpleLocationAsProjectSimpleTree(
        WADNRDbContext dbContext)
    {
        var projects = await dbContext.Projects
            .AsNoTracking()
            .Where(x => x.ProjectLocationPoint == null)
            .Where(IsActiveProjectExpr)
            .OrderBy(x => x.ProjectName)
            .Select(ProjectProjections.AsProjectSimpleTree)
            .ToListAsync();

        return projects;
    }

    private static async Task<Dictionary<int, decimal>> GetTotalTreatedAcresByProjectAsync(WADNRDbContext dbContext)
    {
        var totals = await dbContext.vTotalTreatedAcresByProjects
            .AsNoTracking()
            .ToListAsync();

        return totals.ToDictionary(t => t.ProjectID, t => t.TotalTreatedAcres ?? 0m);
    }

    public static async Task<ProjectMapPopup?> GetByIDAsMapPopupAsync(WADNRDbContext dbContext, int projectID)
    {
        return await dbContext.Projects
            .AsNoTracking()
            .Where(IsActiveProjectExpr)
            .Where(x => x.ProjectID == projectID)
            .Select(ProjectProjections.AsMapPopup)
            .SingleOrDefaultAsync();
    }

    private sealed class ProjectTypeDetailProjectMapPointBaseRow
    {
        public int ProjectID { get; init; }
        public int ProjectStageID { get; init; }
        public int ProjectTypeID { get; init; }
        public int? OrganizationID { get; init; }
        public NetTopologySuite.Geometries.Geometry ProjectLocationPoint { get; init; } = null!;
    }

    public static async Task<FeatureCollection> MapProjectFeatureCollection(IQueryable<Project> projectsThatShouldShowOnMap)
    {
        var baseRows = await projectsThatShouldShowOnMap
            .AsNoTracking()
            .Where(x => x.ProjectLocationPoint != null)
            .Select(x => new ProjectTypeDetailProjectMapPointBaseRow
            {
                ProjectID = x.ProjectID,
                ProjectStageID = x.ProjectStageID,
                ProjectTypeID = x.ProjectTypeID,
                OrganizationID = x.ProjectOrganizations
                    .Where(po => po.RelationshipType.IsPrimaryContact)
                    .Select(po => (int?)po.OrganizationID)
                    .FirstOrDefault(),
                ProjectLocationPoint = x.ProjectLocationPoint!
            })
            .ToListAsync();

        if (baseRows.Count == 0)
        {
            return new FeatureCollection();
        }

        var projectIds = baseRows.Select(r => r.ProjectID).ToList();

        var programPairs = await projectsThatShouldShowOnMap
            .AsNoTracking()
            .Where(p => projectIds.Contains(p.ProjectID))
            .SelectMany(p => p.ProjectPrograms.Select(pp => new { p.ProjectID, pp.ProgramID }))
            .ToListAsync();

        var programCsvByProjectId = programPairs
            .GroupBy(x => x.ProjectID)
            .ToDictionary(
                g => g.Key,
                g => string.Join(",", g.Select(x => x.ProgramID).Distinct().OrderBy(id => id)));

        var classificationPairs = await projectsThatShouldShowOnMap
            .AsNoTracking()
            .Where(p => projectIds.Contains(p.ProjectID))
            .SelectMany(p => p.ProjectClassifications.Select(pc => new { p.ProjectID, pc.ClassificationID }))
            .ToListAsync();

        var classificationCsvByProjectId = classificationPairs
            .GroupBy(x => x.ProjectID)
            .ToDictionary(
                g => g.Key,
                g => string.Join(",", g.Select(x => x.ClassificationID).Distinct().OrderBy(id => id)));

        var featureCollection = new FeatureCollection();

        foreach (var r in baseRows)
        {
            var attributes = new AttributesTable
            {
                { "ProjectID", r.ProjectID },
                { "ProjectStageID", r.ProjectStageID },
                { "ProjectTypeID", r.ProjectTypeID },
                { "OrganizationID", r.OrganizationID ?? 0 },
                { "ProgramID", programCsvByProjectId.TryGetValue(r.ProjectID, out var prog) ? prog : string.Empty },
                { "ClassificationID", classificationCsvByProjectId.TryGetValue(r.ProjectID, out var cls) ? cls : string.Empty },
            };

            featureCollection.Add(new Feature(r.ProjectLocationPoint, attributes));
        }

        return featureCollection;
    }

    public static async Task<List<ProjectClassificationDetailGridRow>> ListAsClassificationDetailGridRowAsync(WADNRDbContext dbContext, int classificationID)
    {
        return await dbContext.ProjectClassifications
            .Where(pc => pc.ClassificationID == classificationID)
            .AsNoTracking()
            .Where(pc => pc.Project.ProjectApprovalStatusID == ApprovedStatusId && !pc.Project.ProjectType.LimitVisibilityToAdmin)
            .OrderBy(pc => pc.Project.ProjectName)
            .Select(ProjectProjections.AsProjectClassificationDetailGridRow)
            .ToListAsync();
    }

    public static async Task<ProjectFactSheet?> GetByIDAsFactSheetAsync(WADNRDbContext dbContext, int projectID)
    {
        var entity = await dbContext.Projects
            .AsNoTracking()
            .Where(IsActiveProjectExpr)
            .Where(x => x.ProjectID == projectID)
            .Select(ProjectProjections.AsFactSheet)
            .SingleOrDefaultAsync();

        if (entity != null)
            entity.DefaultBoundingBox = await GetProjectBoundingBoxAsync(dbContext, projectID);

        return entity;
    }

    public static async Task<List<ClassificationLookupItem>> ListClassificationsAsLookupItemByProjectIDAsync(WADNRDbContext dbContext, int projectID)
    {
        var classifications = await dbContext.Projects
            .AsNoTracking()
            .Where(IsActiveProjectExpr)
            .Where(x => x.ProjectID == projectID)
            .SelectMany(x => x.ProjectClassifications.Select(pc => pc.Classification))
            .Select(c => new ClassificationLookupItem
            {
                ClassificationID = c.ClassificationID,
                DisplayName = c.DisplayName
            })
            .Distinct()
            .OrderBy(x => x.DisplayName)
            .ToListAsync();

        return classifications;
    }

    private static async Task<Dictionary<int, decimal?>> GetTotalFundingByProjectAsync(WADNRDbContext dbContext)
    {
        var totals = await dbContext.ProjectFundSourceAllocationRequests
            .AsNoTracking()
            .GroupBy(r => r.ProjectID)
            .Select(g => new
            {
                ProjectID = g.Key,
                TotalAmount = g.Sum(x => (decimal?)x.TotalAmount)
            })
            .ToListAsync();

        return totals.ToDictionary(x => x.ProjectID, x => x.TotalAmount);
    }

    public static async Task<List<ProjectTagDetailGridRow>> ListAsTagDetailGridRowAsync(WADNRDbContext dbContext, int tagID)
    {
        var rows = await dbContext.Projects
            .Where(p => p.ProjectTags.Any(pt => pt.TagID == tagID))
            .AsNoTracking()
            .Where(IsActiveProjectExpr)
            .OrderBy(x => x.ProjectName)
            .Select(ProjectProjections.AsProjectTagDetailGridRow)
            .ToListAsync();

        var totalsByProjectId = await GetTotalFundingByProjectAsync(dbContext);

        foreach (var r in rows)
        {
            r.TotalAmount = totalsByProjectId.TryGetValue(r.ProjectID, out var total) ? total : null;
        }

        return rows;
    }

    public static async Task<List<ProjectOrganizationDetailGridRow>> ListAsOrganizationDetailGridRowAsync(WADNRDbContext dbContext, int organizationID)
    {
        var rows = await dbContext.Projects
            .Where(p => p.ProjectOrganizations.Any(po => po.OrganizationID == organizationID))
            .AsNoTracking()
            .Where(IsActiveProjectExpr)
            .OrderBy(x => x.ProjectName)
            .Select(ProjectProjections.AsProjectOrganizationDetailGridRow(organizationID))
            .ToListAsync();

        var totalsByProjectId = await GetTotalFundingByProjectAsync(dbContext);

        foreach (var r in rows)
        {
            r.TotalAmount = totalsByProjectId.TryGetValue(r.ProjectID, out var total) ? total : null;
        }

        return rows;
    }

    public static async Task<List<ProjectOrganizationDetailGridRow>> ListPendingAsOrganizationDetailGridRowAsync(WADNRDbContext dbContext, int organizationID)
    {
        var rows = await dbContext.Projects
            .Where(p => p.ProjectOrganizations.Any(po => po.OrganizationID == organizationID))
            .AsNoTracking()
            .Where(p => p.ProjectApprovalStatusID != ApprovedStatusId && !p.ProjectType.LimitVisibilityToAdmin)
            .OrderBy(x => x.ProjectName)
            .Select(ProjectProjections.AsProjectOrganizationDetailGridRow(organizationID))
            .ToListAsync();

        var totalsByProjectId = await GetTotalFundingByProjectAsync(dbContext);

        foreach (var r in rows)
        {
            r.TotalAmount = totalsByProjectId.TryGetValue(r.ProjectID, out var total) ? total : null;
        }

        return rows;
    }

    public static async Task<List<ProjectGridRow>> ListForPersonAsGridRowAsync(WADNRDbContext dbContext, int personID)
    {
        // Get distinct project IDs first to avoid DISTINCT on geometry columns
        var projectIDs = await dbContext.ProjectPeople
            .AsNoTracking()
            .Where(pp => pp.PersonID == personID)
            .Select(pp => pp.ProjectID)
            .Distinct()
            .ToListAsync();

        var projects = await dbContext.Projects
            .AsNoTracking()
            .Where(p => projectIDs.Contains(p.ProjectID))
            .Select(ProjectProjections.AsGridRow)
            .ToListAsync();

        return projects;
    }

    #region User-Aware Methods for Role-Based Visibility

    /// <summary>
    /// Lists all projects visible to the calling user as grid rows.
    /// Applies role-based visibility filtering.
    /// </summary>
    public static async Task<List<ProjectGridRow>> ListAsGridRowForUserAsync(
        WADNRDbContext dbContext,
        PersonDetail? callingUser)
    {
        var query = ProjectVisibility.ApplyVisibilityFilter(dbContext.Projects, callingUser);

        var projects = await query
            .AsNoTracking()
            .OrderBy(x => x.ProjectName)
            .Select(ProjectProjections.AsGridRow)
            .ToListAsync();

        var totalsByProjectId = await GetTotalTreatedAcresByProjectAsync(dbContext);

        foreach (var p in projects)
        {
            p.TotalTreatedAcres = totalsByProjectId.TryGetValue(p.ProjectID, out var total) ? total : 0m;
        }

        return projects;
    }

    /// <summary>
    /// Gets a single project by ID as detail, if visible to the calling user.
    /// Returns null if the project doesn't exist or the user cannot view it.
    /// </summary>
    public static async Task<ProjectDetail?> GetByIDAsDetailForUserAsync(
        WADNRDbContext dbContext,
        int projectID,
        PersonDetail? callingUser)
    {
        var query = ProjectVisibility.ApplyVisibilityFilter(dbContext.Projects, callingUser);

        var entity = await query
            .AsNoTracking()
            .Where(x => x.ProjectID == projectID)
            .Select(ProjectProjections.AsDetail)
            .SingleOrDefaultAsync();

        if (entity == null) return null;

        // Populate FundingSources from the static lookup table
        var fundingSourceIds = await dbContext.ProjectFundingSources
            .AsNoTracking()
            .Where(pfs => pfs.ProjectID == projectID)
            .Select(pfs => pfs.FundingSourceID)
            .ToListAsync();

        entity.FundingSources = fundingSourceIds
            .Select(id => FundingSource.AllLookupDictionary.TryGetValue(id, out var fs) ? fs.FundingSourceDisplayName : null)
            .Where(name => name != null)
            .Cast<string>()
            .OrderBy(name => name)
            .ToList();

        // Populate user permission flags
        PopulateUserPermissionFlags(entity, callingUser, dbContext, projectID);

        // Populate button visibility flags
        await PopulateButtonVisibilityFlagsAsync(entity, callingUser, dbContext, projectID);

        // Populate bounding box using fallback chain
        entity.DefaultBoundingBox = await GetProjectBoundingBoxAsync(dbContext, projectID);

        return entity;
    }

    /// <summary>
    /// Populates the user permission flags on a ProjectDetail based on the calling user's role.
    /// </summary>
    private static void PopulateUserPermissionFlags(
        ProjectDetail entity,
        PersonDetail? callingUser,
        WADNRDbContext dbContext,
        int projectID)
    {
        // Default all to false for anonymous/unassigned users
        if (callingUser == null || callingUser.IsAnonymousOrUnassigned)
        {
            entity.UserCanEdit = false;
            entity.UserCanDirectEdit = false;
            entity.UserCanDelete = false;
            entity.UserCanApprove = false;
            entity.UserIsAdmin = false;
            return;
        }

        // Check if user is admin-level
        var isAdmin = callingUser.BaseRole?.RoleID == (int)RoleEnum.Admin ||
                      callingUser.BaseRole?.RoleID == (int)RoleEnum.EsaAdmin;

        entity.UserIsAdmin = isAdmin || callingUser.HasElevatedProjectAccess;

        // Delete permission: only Admin/EsaAdmin
        entity.UserCanDelete = isAdmin;

        // Approve permission: Admin, EsaAdmin, ProjectSteward, or CanEditProgram
        entity.UserCanApprove = callingUser.HasElevatedProjectAccess || callingUser.HasCanEditProgramRole;

        // Direct edit permission: can bypass the update workflow for notes, photos, documents
        // Matches ProjectEditAsAdminFeature roles (excludes Normal users)
        entity.UserCanDirectEdit = callingUser.HasElevatedProjectAccess || callingUser.HasCanEditProgramRole;

        // Edit permission: depends on project organizations
        // Admin/EsaAdmin/ProjectSteward can edit any project
        if (callingUser.HasElevatedProjectAccess)
        {
            entity.UserCanEdit = true;
        }
        else
        {
            // Normal users can only edit if their org is associated with the project
            var userOrgID = callingUser.OrganizationID;
            if (userOrgID.HasValue)
            {
                var projectOrgIds = entity.Organizations.Select(o => o.OrganizationID).ToList();
                entity.UserCanEdit = projectOrgIds.Contains(userOrgID.Value);
            }
            else
            {
                entity.UserCanEdit = false;
            }
        }
    }

    /// <summary>
    /// Populates button visibility flags on a ProjectDetail.
    /// </summary>
    private static async Task PopulateButtonVisibilityFlagsAsync(
        ProjectDetail entity,
        PersonDetail? callingUser,
        WADNRDbContext dbContext,
        int projectID)
    {
        // CanViewFactSheet: Available for all projects except cancelled
        var projectStageID = entity.ProjectStage?.ProjectStageID;
        entity.CanViewFactSheet = projectStageID != (int)ProjectStageEnum.Cancelled;

        // IsInLandownerAssistanceProgram: Check if project is in Landowner Assistance Program
        entity.IsInLandownerAssistanceProgram = entity.Programs?.Any(p => p.ProgramID == Program.LandownerAssistanceProgramID) ?? false;

        // ExistsInImportBlockList: Check ProjectImportBlockLists table
        entity.ExistsInImportBlockList = await dbContext.ProjectImportBlockLists
            .AnyAsync(x => x.ProjectID == projectID);

        // Check if project is approved (not pending)
        var isApproved = entity.ProjectApprovalStatusID == (int)ProjectApprovalStatusEnum.Approved;

        // Get latest update batch info for this project
        var latestBatch = await dbContext.ProjectUpdateBatches
            .AsNoTracking()
            .Where(b => b.ProjectID == projectID)
            .OrderByDescending(b => b.ProjectUpdateBatchID)
            .Select(b => new
            {
                b.ProjectUpdateBatchID,
                b.ProjectUpdateStateID,
                StateName = b.ProjectUpdateState.ProjectUpdateStateName
            })
            .FirstOrDefaultAsync();

        if (latestBatch != null)
        {
            entity.LatestUpdateBatchStateID = latestBatch.ProjectUpdateStateID;
            entity.LatestUpdateBatchStateName = latestBatch.StateName;

            // HasExistingUpdateBatch: true if there's a batch not in Approved state
            entity.HasExistingUpdateBatch = latestBatch.ProjectUpdateStateID != (int)ProjectUpdateStateEnum.Approved;
        }
        else
        {
            entity.HasExistingUpdateBatch = false;
        }

        // CanStartUpdate: User can edit + project is approved + no existing open batch
        entity.CanStartUpdate = entity.UserCanEdit && isApproved && !entity.HasExistingUpdateBatch;
    }

    /// <summary>
    /// Gets a project's fact sheet by ID, if visible to the calling user.
    /// </summary>
    public static async Task<ProjectFactSheet?> GetByIDAsFactSheetForUserAsync(
        WADNRDbContext dbContext,
        int projectID,
        PersonDetail? callingUser)
    {
        var query = ProjectVisibility.ApplyVisibilityFilter(dbContext.Projects, callingUser);

        var entity = await query
            .AsNoTracking()
            .Where(x => x.ProjectID == projectID)
            .Select(ProjectProjections.AsFactSheet)
            .SingleOrDefaultAsync();

        if (entity != null)
            entity.DefaultBoundingBox = await GetProjectBoundingBoxAsync(dbContext, projectID);

        return entity;
    }

    /// <summary>
    /// Gets a project's map popup by ID, if visible to the calling user.
    /// </summary>
    public static async Task<ProjectMapPopup?> GetByIDAsMapPopupForUserAsync(
        WADNRDbContext dbContext,
        int projectID,
        PersonDetail? callingUser)
    {
        var query = ProjectVisibility.ApplyVisibilityFilter(dbContext.Projects, callingUser);

        return await query
            .AsNoTracking()
            .Where(x => x.ProjectID == projectID)
            .Select(ProjectProjections.AsMapPopup)
            .SingleOrDefaultAsync();
    }

    /// <summary>
    /// Lists projects for a county visible to the calling user.
    /// </summary>
    public static async Task<List<ProjectCountyDetailGridRow>> ListAsCountyDetailGridRowForUserAsync(
        WADNRDbContext dbContext,
        int countyID,
        PersonDetail? callingUser)
    {
        var query = ProjectVisibility.ApplyVisibilityFilter(dbContext.Projects, callingUser);

        return await query
            .Where(p => p.ProjectCounties.Any(pc => pc.CountyID == countyID))
            .AsNoTracking()
            .Select(ProjectProjections.AsProjectCountyDetailGridRow)
            .ToListAsync();
    }

    /// <summary>
    /// Lists projects for a DNR Upland Region visible to the calling user.
    /// </summary>
    public static async Task<List<ProjectDNRUplandRegionDetailGridRow>> ListAsDNRUplandDetailGridRowForUserAsync(
        WADNRDbContext dbContext,
        int dnrUplandRegionID,
        PersonDetail? callingUser)
    {
        var query = ProjectVisibility.ApplyVisibilityFilter(dbContext.Projects, callingUser);

        var rows = await query
            .Where(p => p.ProjectRegions.Any(x => x.DNRUplandRegionID == dnrUplandRegionID))
            .AsNoTracking()
            .OrderBy(x => x.ProjectName)
            .Select(ProjectProjections.AsDnrUplandRegionDetailGridRow)
            .ToListAsync();

        var totalsByProjectId = await GetTotalTreatedAcresByProjectAsync(dbContext);

        foreach (var r in rows)
        {
            r.TotalTreatedAcres = totalsByProjectId.TryGetValue(r.ProjectID, out var total) ? total : 0m;
        }

        return rows;
    }

    /// <summary>
    /// Lists projects for a project type visible to the calling user.
    /// </summary>
    public static async Task<List<ProjectProjectTypeDetailGridRow>> ListAsProjectTypeDetailGridRowForUserAsync(
        WADNRDbContext dbContext,
        int projectTypeID,
        PersonDetail? callingUser)
    {
        var query = ProjectVisibility.ApplyVisibilityFilter(dbContext.Projects, callingUser);

        var rows = await query
            .Where(p => p.ProjectTypeID == projectTypeID)
            .AsNoTracking()
            .OrderBy(x => x.ProjectName)
            .Select(ProjectProjections.AsProjectProjectTypeDetailGridRow)
            .ToListAsync();

        var totalsByProjectId = await GetTotalFundingByProjectAsync(dbContext);

        foreach (var r in rows)
        {
            r.TotalAmount = totalsByProjectId.TryGetValue(r.ProjectID, out var total) ? total : null;
        }

        return rows;
    }

    /// <summary>
    /// Lists projects (approved) for an organization visible to the calling user.
    /// </summary>
    public static async Task<List<ProjectOrganizationDetailGridRow>> ListAsOrganizationDetailGridRowForUserAsync(
        WADNRDbContext dbContext,
        int organizationID,
        PersonDetail? callingUser)
    {
        var query = ProjectVisibility.ApplyVisibilityFilter(dbContext.Projects, callingUser);

        var rows = await query
            .Where(p => p.ProjectOrganizations.Any(po => po.OrganizationID == organizationID))
            .AsNoTracking()
            .OrderBy(x => x.ProjectName)
            .Select(ProjectProjections.AsProjectOrganizationDetailGridRow(organizationID))
            .ToListAsync();

        var totalsByProjectId = await GetTotalFundingByProjectAsync(dbContext);

        foreach (var r in rows)
        {
            r.TotalAmount = totalsByProjectId.TryGetValue(r.ProjectID, out var total) ? total : null;
        }

        return rows;
    }

    /// <summary>
    /// Lists pending projects for an organization, filtered by user visibility.
    /// Only users who are authenticated and either have elevated access or belong to the organization can view.
    /// </summary>
    public static async Task<List<ProjectOrganizationDetailGridRow>> ListPendingAsOrganizationDetailGridRowForUserAsync(
        WADNRDbContext dbContext,
        int organizationID,
        PersonDetail? callingUser)
    {
        var query = ProjectVisibility.ApplyPendingVisibilityFilter(dbContext.Projects, callingUser, organizationID);

        var rows = await query
            .Where(p => p.ProjectOrganizations.Any(po => po.OrganizationID == organizationID))
            .AsNoTracking()
            .OrderBy(x => x.ProjectName)
            .Select(ProjectProjections.AsProjectOrganizationDetailGridRow(organizationID))
            .ToListAsync();

        var totalsByProjectId = await GetTotalFundingByProjectAsync(dbContext);

        foreach (var r in rows)
        {
            r.TotalAmount = totalsByProjectId.TryGetValue(r.ProjectID, out var total) ? total : null;
        }

        return rows;
    }

    /// <summary>
    /// Lists projects for a classification visible to the calling user.
    /// </summary>
    public static async Task<List<ProjectClassificationDetailGridRow>> ListAsClassificationDetailGridRowForUserAsync(
        WADNRDbContext dbContext,
        int classificationID,
        PersonDetail? callingUser)
    {
        var query = ProjectVisibility.ApplyVisibilityFilter(dbContext.Projects, callingUser);

        return await dbContext.ProjectClassifications
            .Where(pc => pc.ClassificationID == classificationID)
            .Where(pc => query.Any(p => p.ProjectID == pc.ProjectID))
            .AsNoTracking()
            .OrderBy(pc => pc.Project.ProjectName)
            .Select(ProjectProjections.AsProjectClassificationDetailGridRow)
            .ToListAsync();
    }

    /// <summary>
    /// Lists projects for a tag visible to the calling user.
    /// </summary>
    public static async Task<List<ProjectTagDetailGridRow>> ListAsTagDetailGridRowForUserAsync(
        WADNRDbContext dbContext,
        int tagID,
        PersonDetail? callingUser)
    {
        var query = ProjectVisibility.ApplyVisibilityFilter(dbContext.Projects, callingUser);

        var rows = await query
            .Where(p => p.ProjectTags.Any(pt => pt.TagID == tagID))
            .AsNoTracking()
            .OrderBy(x => x.ProjectName)
            .Select(ProjectProjections.AsProjectTagDetailGridRow)
            .ToListAsync();

        var totalsByProjectId = await GetTotalFundingByProjectAsync(dbContext);

        foreach (var r in rows)
        {
            r.TotalAmount = totalsByProjectId.TryGetValue(r.ProjectID, out var total) ? total : null;
        }

        return rows;
    }

    /// <summary>
    /// Lists projects with no simple location visible to the calling user.
    /// </summary>
    public static async Task<List<ProjectSimpleTree>> ListWithNoSimpleLocationAsProjectSimpleTreeForUserAsync(
        WADNRDbContext dbContext,
        PersonDetail? callingUser)
    {
        var query = ProjectVisibility.ApplyVisibilityFilter(dbContext.Projects, callingUser);

        return await query
            .Where(x => x.ProjectLocationPoint == null)
            .AsNoTracking()
            .OrderBy(x => x.ProjectName)
            .Select(ProjectProjections.AsProjectSimpleTree)
            .ToListAsync();
    }

    /// <summary>
    /// Creates a GeoJSON feature collection of mapped projects visible to the calling user.
    /// </summary>
    public static async Task<FeatureCollection> MapProjectFeatureCollectionForUser(
        WADNRDbContext dbContext,
        PersonDetail? callingUser)
    {
        var query = ProjectVisibility.ApplyVisibilityFilter(dbContext.Projects, callingUser);
        return await MapProjectFeatureCollection(query);
    }

    /// <summary>
    /// Gets classifications for a specific project, if visible to the calling user.
    /// </summary>
    public static async Task<List<ClassificationLookupItem>> ListClassificationsAsLookupItemByProjectIDForUserAsync(
        WADNRDbContext dbContext,
        int projectID,
        PersonDetail? callingUser)
    {
        var query = ProjectVisibility.ApplyVisibilityFilter(dbContext.Projects, callingUser);

        return await query
            .AsNoTracking()
            .Where(x => x.ProjectID == projectID)
            .SelectMany(x => x.ProjectClassifications.Select(pc => pc.Classification))
            .Select(c => new ClassificationLookupItem
            {
                ClassificationID = c.ClassificationID,
                DisplayName = c.DisplayName
            })
            .Distinct()
            .OrderBy(x => x.DisplayName)
            .ToListAsync();
    }

    /// <summary>
    /// Searches for projects by name or description, filtered by user visibility.
    /// </summary>
    public static async Task<List<ProjectSearchResult>> SearchForUserAsync(
        WADNRDbContext dbContext,
        string searchText,
        PersonDetail? callingUser)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return new List<ProjectSearchResult>();
        }

        var trimmedSearch = searchText.Trim();

        // Apply role-based visibility filter (handles anonymous, elevated, normal users)
        var query = ProjectVisibility.ApplyVisibilityFilter(dbContext.Projects, callingUser);

        return await query
            .AsNoTracking()
            .Where(p => p.ProjectName.Contains(trimmedSearch) ||
                        (p.ProjectDescription != null && p.ProjectDescription.Contains(trimmedSearch)))
            .OrderBy(p => p.ProjectName)
            .Select(p => new ProjectSearchResult
            {
                ProjectID = p.ProjectID,
                ProjectName = p.ProjectName,
                ProjectStageName = p.ProjectStage.ProjectStageName,
                ProjectTypeName = p.ProjectType.ProjectTypeName
            })
            .ToListAsync();
    }

    #endregion

    #region Direct Edit - Save Basics

    public static async Task<ProjectBasicsEditData> GetBasicsEditDataAsync(WADNRDbContext dbContext, int projectID)
    {
        // Get import flags by checking GIS default mappings for the project's programs
        var programIDs = await dbContext.ProjectPrograms
            .Where(pp => pp.ProjectID == projectID)
            .Select(pp => pp.ProgramID)
            .ToListAsync();

        var result = new ProjectBasicsEditData();

        if (programIDs.Count > 0)
        {
            // Get GIS upload source organizations for these programs, with their mappings
            var gisSourceOrgs = await dbContext.GisUploadSourceOrganizations
                .Where(g => programIDs.Contains(g.ProgramID))
                .Include(g => g.GisDefaultMappings)
                .AsNoTracking()
                .ToListAsync();

            foreach (var gisSource in gisSourceOrgs)
            {
                var mappings = gisSource.GisDefaultMappings;

                if (mappings.Any(m => m.FieldDefinitionID == (int)FieldDefinitionEnum.ProjectName && !string.IsNullOrEmpty(m.GisDefaultMappingColumnName)))
                    result.IsProjectNameImported = true;

                if (mappings.Any(m => m.FieldDefinitionID == (int)FieldDefinitionEnum.ProjectStage && !string.IsNullOrEmpty(m.GisDefaultMappingColumnName)))
                    result.IsProjectStageImported = true;

                if (gisSource.ApplyStartDateToProject && mappings.Any(m => m.FieldDefinitionID == (int)FieldDefinitionEnum.PlannedDate && !string.IsNullOrEmpty(m.GisDefaultMappingColumnName)))
                    result.IsProjectInitiationDateImported = true;

                if (gisSource.ApplyCompletedDateToProject && mappings.Any(m => m.FieldDefinitionID == (int)FieldDefinitionEnum.CompletionDate && !string.IsNullOrEmpty(m.GisDefaultMappingColumnName)))
                    result.IsCompletionDateImported = true;

                if (mappings.Any(m => m.FieldDefinitionID == (int)FieldDefinitionEnum.ProjectIdentifier && !string.IsNullOrEmpty(m.GisDefaultMappingColumnName)))
                    result.IsProjectIdentifierImported = true;
            }
        }

        return result;
    }

    public static async Task SaveBasicsAsync(WADNRDbContext dbContext, int projectID, ProjectBasicsSaveRequest request)
    {
        var project = await dbContext.Projects
            .Include(p => p.ProjectPrograms)
            .Include(p => p.ProjectOrganizations)
            .FirstAsync(p => p.ProjectID == projectID);

        project.ProjectTypeID = request.ProjectTypeID;
        project.ProjectName = request.ProjectName;
        project.ProjectDescription = request.ProjectDescription;
        project.ProjectStageID = request.ProjectStageID;
        project.EstimatedTotalCost = request.EstimatedTotalCost;
        project.PlannedDate = request.PlannedDate;
        project.CompletionDate = request.CompletionDate;
        project.ExpirationDate = request.ExpirationDate;
        project.ProjectGisIdentifier = request.ProjectGisIdentifier;
        project.FocusAreaID = request.FocusAreaID;
        project.PercentageMatch = request.PercentageMatch;

        // Sync Lead Implementer Organization via ProjectOrganization with IsPrimaryContact relationship type
        var primaryContactRelationshipTypeID = await dbContext.RelationshipTypes
            .Where(rt => rt.IsPrimaryContact)
            .Select(rt => rt.RelationshipTypeID)
            .FirstAsync();

        var existingLeadImpl = project.ProjectOrganizations
            .FirstOrDefault(po => po.RelationshipTypeID == primaryContactRelationshipTypeID);

        if (request.LeadImplementerOrganizationID.HasValue)
        {
            if (existingLeadImpl != null)
            {
                existingLeadImpl.OrganizationID = request.LeadImplementerOrganizationID.Value;
            }
            else
            {
                dbContext.ProjectOrganizations.Add(new ProjectOrganization
                {
                    ProjectID = projectID,
                    OrganizationID = request.LeadImplementerOrganizationID.Value,
                    RelationshipTypeID = primaryContactRelationshipTypeID
                });
            }
        }
        else if (existingLeadImpl != null)
        {
            dbContext.ProjectOrganizations.Remove(existingLeadImpl);
        }

        // Sync Programs
        var existingProgramIDs = project.ProjectPrograms.Select(pp => pp.ProgramID).ToHashSet();
        var requestedProgramIDs = request.ProgramIDs.ToHashSet();

        // Delete programs not in request
        var toRemove = project.ProjectPrograms.Where(pp => !requestedProgramIDs.Contains(pp.ProgramID)).ToList();
        dbContext.ProjectPrograms.RemoveRange(toRemove);

        // Add new programs
        foreach (var programID in requestedProgramIDs.Except(existingProgramIDs))
        {
            dbContext.ProjectPrograms.Add(new ProjectProgram
            {
                ProjectID = projectID,
                ProgramID = programID
            });
        }

        await dbContext.SaveChangesAsync();
    }

    #endregion

    #region Direct Edit - Location Simple (no auto-assignment)

    public static async Task<LocationSimpleStep?> SaveLocationSimpleAsync(WADNRDbContext dbContext, int projectID, LocationSimpleStepRequest request)
    {
        var project = await dbContext.Projects.FirstOrDefaultAsync(p => p.ProjectID == projectID);
        if (project == null) return null;

        var point = new NetTopologySuite.Geometries.Point(request.Longitude, request.Latitude) { SRID = 4326 };
        project.ProjectLocationPoint = point;
        project.ProjectLocationSimpleTypeID = request.ProjectLocationSimpleTypeID;
        project.ProjectLocationNotes = request.ProjectLocationNotes;

        await dbContext.SaveChangesAsync();

        return await ProjectWorkflowSteps.GetLocationSimpleStepAsync(dbContext, projectID);
    }

    #endregion

    #region Direct Edit - Location Detailed (no auto-assignment)

    public static async Task<LocationDetailedStep?> SaveLocationDetailedAsync(WADNRDbContext dbContext, int projectID, LocationDetailedStepRequest request)
    {
        var project = await dbContext.Projects
            .Include(p => p.ProjectLocations)
                .ThenInclude(pl => pl.Treatments)
            .FirstOrDefaultAsync(p => p.ProjectID == projectID);

        if (project == null) return null;

        var existingLocationIDs = project.ProjectLocations.Select(pl => pl.ProjectLocationID).ToHashSet();
        var requestLocationIDs = request.Locations.Where(l => l.ProjectLocationID.HasValue).Select(l => l.ProjectLocationID!.Value).ToHashSet();

        // Remove locations not in request — guard against deleting locations with treatments
        var toRemove = project.ProjectLocations.Where(pl => !requestLocationIDs.Contains(pl.ProjectLocationID)).ToList();
        var locationsWithTreatments = toRemove.Where(pl => pl.Treatments.Any()).ToList();
        if (locationsWithTreatments.Count > 0)
        {
            var names = string.Join(", ", locationsWithTreatments.Select(pl => $"'{pl.ProjectLocationName}'"));
            throw new InvalidOperationException($"Cannot delete project location(s) {names} because they have associated Treatments. Remove the Treatments first.");
        }

        foreach (var locRequest in request.Locations.Where(l => l.ProjectLocationID.HasValue))
        {
            var existing = project.ProjectLocations.FirstOrDefault(pl => pl.ProjectLocationID == locRequest.ProjectLocationID!.Value);
            if (existing != null && existing.Treatments.Any() && locRequest.ProjectLocationTypeID != existing.ProjectLocationTypeID)
            {
                throw new InvalidOperationException($"Cannot change the location type of '{existing.ProjectLocationName}' because it has associated Treatments.");
            }
        }

        dbContext.ProjectLocations.RemoveRange(toRemove);

        foreach (var locRequest in request.Locations)
        {
            if (locRequest.ProjectLocationID.HasValue && existingLocationIDs.Contains(locRequest.ProjectLocationID.Value))
            {
                var existing = project.ProjectLocations.First(pl => pl.ProjectLocationID == locRequest.ProjectLocationID.Value);
                existing.ProjectLocationTypeID = locRequest.ProjectLocationTypeID;
                existing.ProjectLocationNotes = locRequest.ProjectLocationNotes;
                existing.ProjectLocationName = locRequest.ProjectLocationName;
                if (!string.IsNullOrEmpty(locRequest.GeoJson))
                {
                    var reader = new NetTopologySuite.IO.WKTReader();
                    existing.ProjectLocationGeometry = reader.Read(locRequest.GeoJson);
                    existing.ProjectLocationGeometry.SRID = 4326;
                }
            }
            else
            {
                var newLocation = new ProjectLocation
                {
                    ProjectID = projectID,
                    ProjectLocationTypeID = locRequest.ProjectLocationTypeID,
                    ProjectLocationNotes = locRequest.ProjectLocationNotes,
                    ProjectLocationName = locRequest.ProjectLocationName
                };

                if (!string.IsNullOrEmpty(locRequest.GeoJson))
                {
                    var reader = new NetTopologySuite.IO.WKTReader();
                    newLocation.ProjectLocationGeometry = reader.Read(locRequest.GeoJson);
                    newLocation.ProjectLocationGeometry.SRID = 4326;
                }

                dbContext.ProjectLocations.Add(newLocation);
            }
        }

        await dbContext.SaveChangesAsync();

        return await ProjectWorkflowSteps.GetLocationDetailedStepAsync(dbContext, projectID);
    }

    public static async Task<LocationDetailedStep?> ApproveGdbImportDirectEditAsync(WADNRDbContext dbContext, int projectID, int personID, GdbApproveRequest request)
    {
        var project = await dbContext.Projects.FirstOrDefaultAsync(p => p.ProjectID == projectID);
        if (project == null) return null;

        var stagingRows = await dbContext.ProjectLocationStagings
            .Where(s => s.ProjectID == projectID && s.PersonID == personID)
            .ToListAsync();

        var layerLookup = request.Layers
            .Where(l => l.ShouldImport)
            .ToDictionary(l => l.FeatureClassName, StringComparer.OrdinalIgnoreCase);

        var jsonOptions = new System.Text.Json.JsonSerializerOptions();
        jsonOptions.Converters.Add(new NetTopologySuite.IO.Converters.GeoJsonConverterFactory());

        foreach (var staging in stagingRows)
        {
            if (!layerLookup.TryGetValue(staging.FeatureClassName, out var approval))
            {
                continue;
            }

            var featureCollection = System.Text.Json.JsonSerializer.Deserialize<NetTopologySuite.Features.FeatureCollection>(staging.GeoJson, jsonOptions);
            if (featureCollection == null) continue;

            var locationIndex = 1;
            foreach (var feature in featureCollection)
            {
                var geometry = feature.Geometry;
                if (geometry == null) continue;
                geometry.SRID = 4326;

                string locationName = null;
                if (!string.IsNullOrEmpty(approval.SelectedPropertyName) && feature.Attributes != null)
                {
                    var propValue = feature.Attributes[approval.SelectedPropertyName];
                    if (propValue != null)
                    {
                        locationName = propValue.ToString();
                    }
                }

                if (string.IsNullOrEmpty(locationName))
                {
                    locationName = $"{staging.FeatureClassName} {locationIndex}";
                }

                if (locationName.Length > 100)
                {
                    locationName = locationName.Substring(0, 100);
                }

                dbContext.ProjectLocations.Add(new ProjectLocation
                {
                    ProjectID = projectID,
                    ProjectLocationTypeID = (int)ProjectLocationTypeEnum.ProjectArea,
                    ProjectLocationName = locationName,
                    ProjectLocationGeometry = geometry,
                    ImportedFromGisUpload = true
                });

                locationIndex++;
            }
        }

        dbContext.ProjectLocationStagings.RemoveRange(stagingRows);
        await dbContext.SaveChangesAsync();

        // No AutoAssignGeographicRegionsAsync call — direct edit preserves manual geographic selections
        return await ProjectWorkflowSteps.GetLocationDetailedStepAsync(dbContext, projectID);
    }

    #endregion

    #region Direct Edit - Map Extent

    public static async Task<MapExtentStep?> GetMapExtentAsync(WADNRDbContext dbContext, int projectID)
    {
        var project = await dbContext.Projects
            .AsNoTracking()
            .Where(p => p.ProjectID == projectID)
            .Select(p => new { p.ProjectID, p.DefaultBoundingBox })
            .SingleOrDefaultAsync();

        if (project == null) return null;

        if (project.DefaultBoundingBox != null)
        {
            var env = project.DefaultBoundingBox.EnvelopeInternal;
            return new MapExtentStep
            {
                ProjectID = project.ProjectID,
                North = env.MaxY,
                South = env.MinY,
                East = env.MaxX,
                West = env.MinX
            };
        }

        return new MapExtentStep { ProjectID = project.ProjectID };
    }

    public static async Task SaveMapExtentAsync(WADNRDbContext dbContext, int projectID, MapExtentSaveRequest request)
    {
        var project = await dbContext.Projects.FirstAsync(p => p.ProjectID == projectID);

        if (request.North.HasValue && request.South.HasValue && request.East.HasValue && request.West.HasValue)
        {
            var envelope = new NetTopologySuite.Geometries.Envelope(
                request.West.Value, request.East.Value,
                request.South.Value, request.North.Value);
            var factory = new NetTopologySuite.Geometries.GeometryFactory(new NetTopologySuite.Geometries.PrecisionModel(), 4326);
            project.DefaultBoundingBox = factory.ToGeometry(envelope);
        }
        else
        {
            project.DefaultBoundingBox = null;
        }

        await dbContext.SaveChangesAsync();
    }

    #endregion

    #region Bounding Box Fallback Chain

    public static async Task<BoundingBox?> GetProjectBoundingBoxAsync(WADNRDbContext dbContext, int projectID)
    {
        var project = await dbContext.Projects
            .AsNoTracking()
            .Include(p => p.ProjectLocations)
            .Include(p => p.ProjectRegions).ThenInclude(pr => pr.DNRUplandRegion)
            .Include(p => p.ProjectPriorityLandscapes).ThenInclude(ppl => ppl.PriorityLandscape)
            .Where(p => p.ProjectID == projectID)
            .SingleOrDefaultAsync();

        if (project == null) return null;

        // 1. Custom DefaultBoundingBox
        if (project.DefaultBoundingBox != null)
            return GeometryToBoundingBox(project.DefaultBoundingBox);

        // 2. Detailed locations envelope
        var locationGeometries = project.ProjectLocations
            .Select(pl => pl.ProjectLocationGeometry)
            .Where(g => g != null).ToList();
        if (locationGeometries.Count > 0)
            return GeometryListToBoundingBox(locationGeometries);

        // 3. Simple location point with padding
        if (project.ProjectLocationPoint != null)
            return PointToPaddedBoundingBox(project.ProjectLocationPoint, 0.001);

        // 4. Regions + Priority Landscapes
        var areaGeometries = project.ProjectRegions
            .Select(pr => pr.DNRUplandRegion.DNRUplandRegionLocation)
            .Where(g => g != null)
            .Concat(project.ProjectPriorityLandscapes
                .Select(ppl => ppl.PriorityLandscape.PriorityLandscapeLocation)
                .Where(g => g != null))
            .ToList();
        if (areaGeometries.Count > 0)
            return GeometryListToBoundingBox(areaGeometries);

        // 5. null - frontend uses default WA state bounds
        return null;
    }

    private static BoundingBox GeometryToBoundingBox(Geometry geometry)
    {
        var env = geometry.EnvelopeInternal;
        return new BoundingBox
        {
            Left = env.MinX,
            Bottom = env.MinY,
            Right = env.MaxX,
            Top = env.MaxY
        };
    }

    private static BoundingBox GeometryListToBoundingBox(List<Geometry> geometries)
    {
        var envelope = new Envelope();
        foreach (var g in geometries)
        {
            envelope.ExpandToInclude(g.EnvelopeInternal);
        }
        return new BoundingBox
        {
            Left = envelope.MinX,
            Bottom = envelope.MinY,
            Right = envelope.MaxX,
            Top = envelope.MaxY
        };
    }

    private static BoundingBox PointToPaddedBoundingBox(Geometry point, double padding)
    {
        var coord = point.Coordinate;
        return new BoundingBox
        {
            Left = coord.X - padding,
            Bottom = coord.Y - padding,
            Right = coord.X + padding,
            Top = coord.Y + padding
        };
    }

    #endregion
}
