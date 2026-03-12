using Microsoft.EntityFrameworkCore;

namespace WADNR.EFModels.Entities;

/// <summary>
/// Lightweight data class holding only what's needed for authorization checks on a project.
/// Loaded once per request by ProjectAuthorizationContext, reused by auth attributes and controllers.
/// </summary>
public class ProjectAuthorizationData
{
    public int ProjectID { get; set; }
    public int ProjectApprovalStatusID { get; set; }
    public int? StewardingOrganizationID { get; set; }
    public int? TaxonomyBranchID { get; set; }
    public List<int> RegionIDs { get; set; } = [];
    public List<int> ProgramIDs { get; set; } = [];
    public int? PrimaryContactPersonID { get; set; }
    public int? LeadImplementerOrganizationID { get; set; }
    public int? ProposingPersonID { get; set; }
    public int? ProposingPersonOrganizationID { get; set; }

    public bool IsPendingProject =>
        ProjectApprovalStatusID is (int)ProjectApprovalStatusEnum.Draft
            or (int)ProjectApprovalStatusEnum.PendingApproval
            or (int)ProjectApprovalStatusEnum.Returned
            or (int)ProjectApprovalStatusEnum.Rejected;

    public static ProjectAuthorizationData? Load(WADNRDbContext dbContext, int projectID)
    {
        return LoadCore(dbContext, projectID).GetAwaiter().GetResult();
    }

    public static async Task<ProjectAuthorizationData?> LoadAsync(WADNRDbContext dbContext, int projectID)
    {
        return await LoadCore(dbContext, projectID);
    }

    private static async Task<ProjectAuthorizationData?> LoadCore(WADNRDbContext dbContext, int projectID)
    {
        var project = await dbContext.Projects
            .AsNoTracking()
            .Where(p => p.ProjectID == projectID)
            .Select(p => new ProjectAuthorizationData
            {
                ProjectID = p.ProjectID,
                ProjectApprovalStatusID = p.ProjectApprovalStatusID,
                StewardingOrganizationID = p.ProjectOrganizations
                    .Where(po => po.RelationshipType.CanStewardProjects)
                    .Select(po => (int?)po.OrganizationID)
                    .SingleOrDefault(),
                TaxonomyBranchID = p.ProjectType.TaxonomyBranchID,
                RegionIDs = p.ProjectRegions.Select(pr => pr.DNRUplandRegionID).ToList(),
                ProgramIDs = p.ProjectPrograms.Select(pp => pp.ProgramID).ToList(),
                LeadImplementerOrganizationID = p.ProjectOrganizations
                    .Where(po => po.RelationshipType.IsPrimaryContact)
                    .Select(po => (int?)po.OrganizationID)
                    .SingleOrDefault(),
                ProposingPersonID = p.ProposingPersonID,
                ProposingPersonOrganizationID = p.ProposingPerson != null ? p.ProposingPerson.OrganizationID : null,
            })
            .SingleOrDefaultAsync();

        if (project == null) return null;

        // PrimaryContactPersonID comes from ProjectPeople where relationship type is PrimaryContact (ID=1)
        // ProjectPersonRelationshipType is a lookup binding (not DB FK), so use the ID directly
        project.PrimaryContactPersonID = await dbContext.ProjectPeople
            .AsNoTracking()
            .Where(pp => pp.ProjectID == projectID
                && pp.ProjectPersonRelationshipTypeID == (int)ProjectPersonRelationshipTypeEnum.PrimaryContact)
            .Select(pp => (int?)pp.PersonID)
            .SingleOrDefaultAsync();

        return project;
    }
}
