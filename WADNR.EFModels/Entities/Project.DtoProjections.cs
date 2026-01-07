using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ProjectProjections
{
    public static readonly Expression<Func<Project, ProjectDetail>> AsDetail = x => new ProjectDetail
    {
        ProjectID = x.ProjectID,
        ProjectName = x.ProjectName,
        ProjectDescription = x.ProjectDescription,
        PlannedDate = x.PlannedDate,
        CompletionDate = x.CompletionDate,
        EstimatedTotalCost = x.EstimatedTotalCost,
        FhtProjectNumber = x.FhtProjectNumber
    };

    public static readonly Expression<Func<Project, ProjectGridRow>> AsGridRow = x => new ProjectGridRow
    {
        ProjectID = x.ProjectID,
        ProjectName = x.ProjectName,
        FhtProjectNumber = x.FhtProjectNumber,
        ProjectType = new ProjectTypeLookupItem
        {
            ProjectTypeID = x.ProjectType.ProjectTypeID,
            ProjectTypeName = x.ProjectType.ProjectTypeName
        },
        ProjectStage = new ProjectStageLookupItem
        {
            ProjectStageID = x.ProjectStageID,
            ProjectStageName = x.ProjectStage.ProjectStageName
        },
        LeadImplementerOrganization = x.ProjectOrganizations
            .Where(po => po.RelationshipType.IsPrimaryContact)
            .Select(po => new OrganizationLookupItem
            {
                OrganizationID = po.Organization.OrganizationID,
                OrganizationName = po.Organization.DisplayName
            })
            .SingleOrDefault(),
        Programs = x.ProjectPrograms
            .Where(pp => !pp.Program.IsDefaultProgramForImportOnly)
            .Select(pp => new ProgramLookupItem
            {
                ProgramID = pp.Program.ProgramID,
                ProgramName = pp.Program.DisplayName
            })
            .ToList(),
        PriorityLandscape = x.ProjectPriorityLandscapes
            .Select(ppl => new PriorityLandscapeLookupItem
            {
                PriorityLandscapeID = ppl.PriorityLandscape.PriorityLandscapeID,
                PriorityLandscapeName = ppl.PriorityLandscape.PriorityLandscapeName
            })
            .FirstOrDefault(),
        County = x.ProjectCounties
            .Select(pc => new CountyLookupItem
            {
                CountyID = pc.County.CountyID,
                CountyName = pc.County.CountyName
            })
            .FirstOrDefault()
    };

    public static readonly Expression<Func<Project, ProjectCountyDetailGridRow>> AsProjectCountyDetailGridRow = x => new ProjectCountyDetailGridRow
    {
        ProjectID = x.ProjectID,
        ProjectName = x.ProjectName,
        FhtProjectNumber = x.FhtProjectNumber,
        PrimaryContactOrganization = x.ProjectOrganizations
            .Where(po => po.RelationshipType.IsPrimaryContact)
            .Select(po => new OrganizationLookupItem
            {
                OrganizationID = po.Organization.OrganizationID,
                OrganizationName = po.Organization.DisplayName
            })
            .SingleOrDefault(),
        ProjectStage = new ProjectStageLookupItem
        {
            ProjectStageID = x.ProjectStageID,
            ProjectStageName = x.ProjectStage.ProjectStageName
        },
        ProjectInitiationDate = x.PlannedDate,
        ExpirationDate = x.ExpirationDate,
        CompletionDate = x.CompletionDate,
        EstimatedTotalCost = x.EstimatedTotalCost,
        TotalAmount = x.ProjectFundSourceAllocationRequests.Any()
            ? x.ProjectFundSourceAllocationRequests.Sum(r => (decimal?)r.TotalAmount)
            : null,
        ProjectDescription = x.ProjectDescription
    };
}
