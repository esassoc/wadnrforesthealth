using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.ProjectUpdate;

namespace WADNR.EFModels.Entities;

/// <summary>
/// Intermediate row for the AsFeaturedRaw projection. Fields that can't translate to SQL
/// (string.Join for Implementers, Duration formatting) are resolved in memory in ListFeaturedAsync.
/// </summary>
public class ProjectFeaturedRaw
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectNumber { get; set; } = string.Empty;
    public string? ActionPriority { get; set; }
    public List<string> Implementers { get; set; } = new();
    public string Stage { get; set; } = string.Empty;
    public int? PlannedYear { get; set; }
    public int? CompletionYear { get; set; }
    public string ProjectDescription { get; set; } = string.Empty;
    public Guid? KeyPhotoFileResourceGuid { get; set; }
    public string? KeyPhotoCaption { get; set; }
}

public static class ProjectProjections
{
    public static readonly Expression<Func<Project, ProjectDetail>> AsDetail = x => new ProjectDetail
    {
        // Core identifiers
        ProjectID = x.ProjectID,
        ProjectName = x.ProjectName,
        FhtProjectNumber = x.FhtProjectNumber,
        ProjectGisIdentifier = x.ProjectGisIdentifier,

        // Type and Stage
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

        // Approval Status
        ProjectApprovalStatusID = x.ProjectApprovalStatusID,
        ProjectApprovalStatusName = x.ProjectApprovalStatus.ProjectApprovalStatusDisplayName,

        // Dates
        PlannedDate = x.PlannedDate,
        CompletionDate = x.CompletionDate,
        ExpirationDate = x.ExpirationDate,
        Duration = x.Duration,

        // Description
        ProjectDescription = x.ProjectDescription,

        // Financial
        EstimatedTotalCost = x.EstimatedTotalCost,
        PercentageMatch = x.PercentageMatch,

        // Focus Area
        FocusAreaID = x.FocusAreaID,
        FocusAreaName = x.FocusArea != null ? x.FocusArea.FocusAreaName : null,

        // Lead Implementer
        LeadImplementer = x.ProjectOrganizations
            .Where(po => po.RelationshipType.IsPrimaryContact)
            .Select(po => new OrganizationLookupItem
            {
                OrganizationID = po.Organization.OrganizationID,
                OrganizationName = po.Organization.DisplayName
            })
            .FirstOrDefault(),

        // Programs
        Programs = x.ProjectPrograms
            .Where(pp => !pp.Program.IsDefaultProgramForImportOnly)
            .Select(pp => new ProgramLookupItem
            {
                ProgramID = pp.Program.ProgramID,
                ProgramName = pp.Program.DisplayName
            })
            .ToList(),

        // Organizations
        Organizations = x.ProjectOrganizations
            .Select(po => new ProjectOrganizationItem
            {
                ProjectOrganizationID = po.ProjectOrganizationID,
                OrganizationID = po.Organization.OrganizationID,
                OrganizationName = po.Organization.DisplayName,
                RelationshipTypeID = po.RelationshipTypeID,
                RelationshipTypeName = po.RelationshipType.RelationshipTypeName,
                IsPrimaryContact = po.RelationshipType.IsPrimaryContact
            })
            .ToList(),

        // People
        People = x.ProjectPeople
            .Select(pp => new ProjectPersonItem
            {
                ProjectPersonID = pp.ProjectPersonID,
                PersonID = pp.Person.PersonID,
                PersonFullName = pp.Person.FirstName + " " + pp.Person.LastName,
                RelationshipTypeID = pp.ProjectPersonRelationshipTypeID,
                RelationshipTypeName = pp.ProjectPersonRelationshipType.ProjectPersonRelationshipTypeDisplayName,
                SortOrder = pp.ProjectPersonRelationshipType.SortOrder
            })
            .ToList(),

        // Tags
        Tags = x.ProjectTags
            .Select(pt => new TagLookupItem
            {
                TagID = pt.Tag.TagID,
                TagName = pt.Tag.TagName
            })
            .ToList(),

        // Classifications
        Classifications = x.ProjectClassifications
            .Select(pc => new ClassificationLookupItem
            {
                ClassificationID = pc.Classification.ClassificationID,
                DisplayName = pc.Classification.DisplayName
            })
            .ToList(),

        // Funding Source Notes
        FundingSourceNotes = x.ProjectFundingSourceNotes,

        // Fund Source Allocation Requests
        FundSourceAllocationRequests = x.ProjectFundSourceAllocationRequests
            .Select(r => new FundSourceAllocationRequestItem
            {
                ProjectFundSourceAllocationRequestID = r.ProjectFundSourceAllocationRequestID,
                FundSourceAllocationID = r.FundSourceAllocationID,
                FundSourceAllocationName = r.FundSourceAllocation.FundSourceAllocationName ?? string.Empty,
                FundSourceName = r.FundSourceAllocation.FundSource.FundSourceName,
                MatchAmount = r.MatchAmount,
                PayAmount = r.PayAmount,
                TotalAmount = r.TotalAmount
            })
            .ToList(),

        // Associated Agreements
        Agreements = x.AgreementProjects
            .Select(ap => new AgreementLookupItem
            {
                AgreementID = ap.Agreement.AgreementID,
                AgreementTitle = ap.Agreement.AgreementTitle ?? string.Empty,
                AgreementNumber = ap.Agreement.AgreementNumber
            })
            .ToList(),

        // Location
        HasLocationData = x.ProjectLocationPoint != null || x.ProjectLocations.Any(),
        Latitude = x.ProjectLocationPoint != null ? x.ProjectLocationPoint.Coordinate.Y : (double?)null,
        Longitude = x.ProjectLocationPoint != null ? x.ProjectLocationPoint.Coordinate.X : (double?)null,
        ProjectLocationNotes = x.ProjectLocationNotes,
        Counties = x.ProjectCounties.Select(pc => new CountyLookupItem
        {
            CountyID = pc.County.CountyID,
            CountyName = pc.County.CountyName
        }).ToList(),
        Regions = x.ProjectRegions.Select(pr => new DNRUplandRegionLookupItem
        {
            DNRUplandRegionID = pr.DNRUplandRegion.DNRUplandRegionID,
            DNRUplandRegionName = pr.DNRUplandRegion.DNRUplandRegionName
        }).ToList(),
        PriorityLandscapes = x.ProjectPriorityLandscapes.Select(ppl => new PriorityLandscapeLookupItem
        {
            PriorityLandscapeID = ppl.PriorityLandscape.PriorityLandscapeID,
            PriorityLandscapeName = ppl.PriorityLandscape.PriorityLandscapeName
        }).ToList()
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
        ProjectApprovalStatusID = x.ProjectApprovalStatusID,
        ProjectApprovalStatusName = x.ProjectApprovalStatus.ProjectApprovalStatusDisplayName,
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

    public static readonly Expression<Func<Project, PendingProjectGridRow>> AsPendingGridRow = x => new PendingProjectGridRow
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
        ProjectApprovalStatusID = x.ProjectApprovalStatusID,
        ProjectApprovalStatusName = x.ProjectApprovalStatus.ProjectApprovalStatusDisplayName,
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
            .FirstOrDefault(),
        ProjectInitiationDate = x.PlannedDate,
        ExpirationDate = x.ExpirationDate,
        CompletionDate = x.CompletionDate,
        EstimatedTotalCost = x.EstimatedTotalCost,
        TotalAmount = x.ProjectFundSourceAllocationRequests.Any()
            ? x.ProjectFundSourceAllocationRequests.Sum(r => (decimal?)r.TotalAmount)
            : null,
        SubmittedDate = x.SubmissionDate,
        LastUpdatedDate = null, // Populated in-memory from ProjectUpdateBatches
        ProjectDescription = x.ProjectDescription
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

    public static readonly Expression<Func<Project, ProjectTagDetailGridRow>> AsProjectTagDetailGridRow = x => new ProjectTagDetailGridRow
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

    // Intermediate row for featured projects — SQL-translatable fields only.
    // string.Join (Implementers) and Duration formatting happen in memory in ListFeaturedAsync.
    public static readonly Expression<Func<Project, ProjectFeaturedRaw>> AsFeaturedRaw = x => new ProjectFeaturedRaw
    {
        ProjectID = x.ProjectID,
        ProjectName = x.ProjectName,
        ProjectNumber = x.FhtProjectNumber,
        ActionPriority = x.ProjectType.TaxonomyBranch.TaxonomyTrunk.TaxonomyTrunkName,
        Implementers = x.ProjectOrganizations
            .Where(po => po.RelationshipType.IsPrimaryContact)
            .Select(po => po.Organization.DisplayName)
            .ToList(),
        Stage = x.ProjectStage.ProjectStageName,
        PlannedYear = x.PlannedDate != null ? (int?)x.PlannedDate.Value.Year : null,
        CompletionYear = x.CompletionDate != null ? (int?)x.CompletionDate.Value.Year : null,
        ProjectDescription = x.ProjectDescription ?? string.Empty,
        KeyPhotoFileResourceGuid = x.ProjectImages
            .Where(pi => pi.IsKeyPhoto)
            .Select(pi => (Guid?)pi.FileResource.FileResourceGUID)
            .FirstOrDefault(),
        KeyPhotoCaption = x.ProjectImages
            .Where(pi => pi.IsKeyPhoto)
            .Select(pi => pi.Caption)
            .FirstOrDefault(),
    };

    public static readonly Expression<Func<Project, ProjectLookupItem>> AsLookupItem = x => new ProjectLookupItem
    {
        ProjectID = x.ProjectID,
        ProjectName = x.ProjectName
    };

    public static readonly Expression<Func<Project, ProjectUpdateStatusGridRow>> AsUpdateStatusGridRow = p => new ProjectUpdateStatusGridRow
    {
        ProjectID = p.ProjectID,
        ProjectName = p.ProjectName,
        FhtProjectNumber = p.FhtProjectNumber,
        ProjectStageName = p.ProjectStage.ProjectStageName,
        LeadImplementerOrganizationName = p.ProjectOrganizations
            .Where(po => po.RelationshipType.IsPrimaryContact)
            .Select(po => po.Organization.OrganizationName)
            .FirstOrDefault(),
        EstimatedTotalCost = p.EstimatedTotalCost,
        ProjectUpdateBatchID = p.ProjectUpdateBatches
            .OrderByDescending(b => b.LastUpdateDate)
            .Select(b => (int?)b.ProjectUpdateBatchID)
            .FirstOrDefault(),
        ProjectUpdateStateID = p.ProjectUpdateBatches
            .OrderByDescending(b => b.LastUpdateDate)
            .Select(b => (int?)b.ProjectUpdateStateID)
            .FirstOrDefault(),
        ProjectUpdateStateName = null, // Resolved client-side
        LastUpdateDate = p.ProjectUpdateBatches
            .OrderByDescending(b => b.LastUpdateDate)
            .Select(b => (DateTime?)b.LastUpdateDate)
            .FirstOrDefault(),
        LastUpdatedByPersonName = p.ProjectUpdateBatches
            .OrderByDescending(b => b.LastUpdateDate)
            .Select(b => b.LastUpdatePerson.FirstName + " " + b.LastUpdatePerson.LastName)
            .FirstOrDefault(),
        IsMyProject = false, // Resolved in static helper
    };

    public static Expression<Func<Project, ProjectOrganizationDetailGridRow>> AsProjectOrganizationDetailGridRow(int organizationID) => x => new ProjectOrganizationDetailGridRow
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
        ProjectStewardOrganization = x.ProjectOrganizations
            .Where(po => po.RelationshipType.CanStewardProjects)
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
        RelationshipTypes = string.Join(", ", x.ProjectOrganizations
            .Where(po => po.OrganizationID == organizationID)
            .Select(po => po.RelationshipType.RelationshipTypeName)),
        ProjectInitiationDate = x.PlannedDate,
        ExpirationDate = x.ExpirationDate,
        CompletionDate = x.CompletionDate,
        EstimatedTotalCost = x.EstimatedTotalCost,
        TotalAmount = null,
        ProjectDescription = x.ProjectDescription,
        PhotoCount = x.ProjectImages.Count
    };
}
