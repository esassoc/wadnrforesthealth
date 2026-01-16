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
        FhtProjectNumber = x.FhtProjectNumber,
    };

    public static readonly Expression<Func<Project, ProjectMapPopup>> AsMapPopup = x => new ProjectMapPopup
    {
        ProjectID = x.ProjectID,
        ProjectName = x.ProjectName,
        Duration = x.Duration,
        ProjectType = new ProjectTypeLookupItem
        {
            ProjectTypeID = x.ProjectType.ProjectTypeID,
            ProjectTypeName = x.ProjectType.ProjectTypeName
        },
        ProjectStage = new ProjectStageLookupItem
        {
            ProjectStageID = x.ProjectStage.ProjectStageID,
            ProjectStageName = x.ProjectStage.ProjectStageName
        },
        LeadImplementer = x.ProjectOrganizations
            .Where(po => po.RelationshipType.IsPrimaryContact)
            .Select(po => new OrganizationLookupItem
            {
                OrganizationID = po.Organization.OrganizationID,
                OrganizationName = po.Organization.DisplayName
            })
            .SingleOrDefault(),
        Classifications = x.ProjectClassifications
            .Select(pc => new ClassificationLookupItem
            {
                ClassificationID = pc.Classification.ClassificationID,
                DisplayName = pc.Classification.DisplayName
            }).ToList()
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

    public static readonly Expression<Func<Project, ProjectProjectTypeDetailGridRow>> AsProjectProjectTypeDetailGridRow = x => new ProjectProjectTypeDetailGridRow
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
        TotalAmount = null,
        ProjectDescription = x.ProjectDescription
    };

    public static readonly Expression<Func<Project, ProjectDNRUplandRegionDetailGridRow>> AsDnrUplandRegionDetailGridRow = x => new ProjectDNRUplandRegionDetailGridRow
    {
        ProjectID = x.ProjectID,
        ProjectName = x.ProjectName,
        LeadImplementer = x.ProjectOrganizations
            .Where(po => po.RelationshipType.IsPrimaryContact)
            .Select(po => new OrganizationLookupItem
            {
                OrganizationID = po.Organization.OrganizationID,
                OrganizationName = po.Organization.DisplayName
            })
            .SingleOrDefault(),
        Programs = x.ProjectPrograms
            .Select(pp => new ProgramLookupItem
            {
                ProgramID = pp.Program.ProgramID,
                ProgramName = pp.Program.IsDefaultProgramForImportOnly
                    ? pp.Program.Organization.DisplayName
                    : (
                        pp.Program.ProgramName
                        + (!string.IsNullOrWhiteSpace(pp.Program.ProgramShortName) ? " (" + pp.Program.ProgramShortName + ")" : string.Empty)
                        + (!pp.Program.ProgramIsActive ? " (Inactive)" : string.Empty)
                      )
            }).ToList(),
        Counties = x.ProjectCounties
            .Select(pc => new CountyLookupItem
            {
                CountyID = pc.County.CountyID,
                CountyName = pc.County.CountyName
            }).ToList(),
        PrimaryContact = x.ProjectPeople
            .Where(pp => pp.ProjectPersonRelationshipTypeID == ProjectPersonRelationshipType.PrimaryContact.ProjectPersonRelationshipTypeID)
            .Select(pp => new PersonLookupItem
            {
                PersonID = pp.Person.PersonID,
                FullName = pp.Person.FirstName + " " + pp.Person.LastName
            })
            .SingleOrDefault()
            ?? x.ProjectOrganizations
                .Where(po => po.RelationshipType.IsPrimaryContact && po.Organization.PrimaryContactPerson != null)
                .Select(po => new PersonLookupItem
                {
                    PersonID = po.Organization.PrimaryContactPerson!.PersonID,
                    FullName = po.Organization.PrimaryContactPerson!.FirstName + " " + po.Organization.PrimaryContactPerson!.LastName
                })
                .FirstOrDefault(),
        TotalTreatedAcres = x.Treatments.Sum(t => (decimal?)(t.TreatmentTreatedAcres ?? 0)),
        ProjectType = new ProjectTypeLookupItem
        {
            ProjectTypeID = x.ProjectType.ProjectTypeID,
            ProjectTypeName = x.ProjectType.ProjectTypeName
        },
        ProjectStage = new ProjectStageLookupItem
        {
            ProjectStageID = x.ProjectStage.ProjectStageID,
            ProjectStageName = x.ProjectStage.ProjectStageName
        },
        ProjectApplicationDate = x.SubmissionDate,
        ProjectInitiationDate = x.PlannedDate,
        ProjectExpiryDate = x.ExpirationDate,
        ProjectCompletionDate = x.CompletionDate,
        TotalPaymentAmount = x.InvoicePaymentRequests.SelectMany(ipr => ipr.Invoices).Sum(inv => inv.PaymentAmount ?? 0),
        TotalMatchAmount = x.InvoicePaymentRequests.SelectMany(ipr => ipr.Invoices).Sum(inv => inv.MatchAmount ?? 0),
        PercentageMatch = x.PercentageMatch,
        ExpectedFundingFundSourceAllocations = x.ProjectFundSourceAllocationRequests
            .Select(r => new FundSourceAllocationLookupItem
            {
                FundSourceAllocationID = r.FundSourceAllocation.FundSourceAllocationID,
                FundSourceAllocationName = r.FundSourceAllocation.FundSourceAllocationName
            }).ToList()
    };

    public static readonly Expression<Func<Project, ProjectSimpleTree>> AsProjectSimpleTree = x => new ProjectSimpleTree
    {
        ProjectID = x.ProjectID,
        ProjectName = x.ProjectName,
        ProjectType = new ProjectTypeLookupItem
        {
            ProjectTypeID = x.ProjectType.ProjectTypeID,
            ProjectTypeName = x.ProjectType.ProjectTypeName
        }
    };

    public static readonly Expression<Func<ProjectClassification, ProjectClassificationDetailGridRow>> AsProjectClassificationDetailGridRow = x => new ProjectClassificationDetailGridRow
    {
        ProjectID = x.ProjectID,
        ProjectName = x.Project.ProjectName,
        FhtProjectNumber = x.Project.FhtProjectNumber,
        PrimaryContactOrganization = x.Project.ProjectOrganizations
            .Where(po => po.RelationshipType.IsPrimaryContact)
            .Select(po => new OrganizationLookupItem
            {
                OrganizationID = po.Organization.OrganizationID,
                OrganizationName = po.Organization.DisplayName
            })
            .SingleOrDefault(),
        ProjectStage = new ProjectStageLookupItem
        {
            ProjectStageID = x.Project.ProjectStageID,
            ProjectStageName = x.Project.ProjectStage.ProjectStageName
        },
        ProjectInitiationDate = x.Project.PlannedDate,
        ProjectThemeNotes = x.ProjectClassificationNotes
    };

    public static readonly Expression<Func<Project, ProjectFactSheet>> AsFactSheet = x => new ProjectFactSheet
    {
        ProjectID = x.ProjectID,
        ProjectName = x.ProjectName,
        ProjectDescription = x.ProjectDescription,
        ProjectType = new ProjectTypeLookupItemWithColor
        {
            ProjectTypeID = x.ProjectType.ProjectTypeID,
            ProjectTypeName = x.ProjectType.ProjectTypeName,
            ThemeColor = x.ProjectType.ThemeColor
        },
        LeadImplementer = x.ProjectOrganizations
            .Where(po => po.RelationshipType.IsPrimaryContact)
            .Select(po => new OrganizationLookupItem
            {
                OrganizationID = po.Organization.OrganizationID,
                OrganizationName = po.Organization.DisplayName
            })
            .SingleOrDefault()!,
        PrimaryContact = x.ProjectPeople
            .Where(pp => pp.ProjectPersonRelationshipTypeID == ProjectPersonRelationshipType.PrimaryContact.ProjectPersonRelationshipTypeID)
            .Select(pp => new PersonLookupItemWithEmail()
            {
                PersonID = pp.Person.PersonID,
                FullName = pp.Person.FirstName + " " + pp.Person.LastName,
                Email = pp.Person.Email
            })
            .SingleOrDefault()!,
        ProjectStage = new ProjectStageLookupItem
        {
            ProjectStageID = x.ProjectStage.ProjectStageID,
            ProjectStageName = x.ProjectStage.ProjectStageName
        },
        Duration = x.Duration
    };
}
