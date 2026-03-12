namespace WADNR.Models.DataTransferObjects;

public class ProjectDetail
{
    // Core identifiers
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? FhtProjectNumber { get; set; }
    public string? ProjectGisIdentifier { get; set; }

    // Type and Stage
    public ProjectTypeLookupItem? ProjectType { get; set; }
    public ProjectStageLookupItem? ProjectStage { get; set; }

    // Approval Status
    public int ProjectApprovalStatusID { get; set; }
    public string? ProjectApprovalStatusName { get; set; }

    // Dates
    public DateTime? PlannedDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? Duration { get; set; }

    // Description and Notes
    public string? ProjectDescription { get; set; }

    // Financial
    public decimal? EstimatedTotalCost { get; set; }
    public decimal? PercentageMatch { get; set; }

    // Focus Area
    public int? FocusAreaID { get; set; }
    public string? FocusAreaName { get; set; }

    // Lead Implementer
    public OrganizationLookupItem? LeadImplementer { get; set; }

    // Programs
    public List<ProgramLookupItem> Programs { get; set; } = new();

    // Organizations with relationship types
    public List<ProjectOrganizationItem> Organizations { get; set; } = new();

    // People/Contacts with relationship types
    public List<ProjectPersonItem> People { get; set; } = new();

    // Tags
    public List<TagLookupItem> Tags { get; set; } = new();

    // Classifications (for themes)
    public List<ClassificationLookupItem> Classifications { get; set; } = new();

    // Funding
    public List<string> FundingSources { get; set; } = new();
    public string? FundingSourceNotes { get; set; }
    public List<FundSourceAllocationRequestItem> FundSourceAllocationRequests { get; set; } = new();

    // Associated Agreements
    public List<AgreementLookupItem> Agreements { get; set; } = new();

    // Location
    public BoundingBox? DefaultBoundingBox { get; set; }
    public bool HasLocationData { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? ProjectLocationNotes { get; set; }
    public List<CountyLookupItem> Counties { get; set; } = new();
    public List<DNRUplandRegionLookupItem> Regions { get; set; } = new();
    public List<PriorityLandscapeLookupItem> PriorityLandscapes { get; set; } = new();

    // User-specific permission flags (computed server-side per calling user)
    public bool UserCanEdit { get; set; }
    public bool UserCanDirectEdit { get; set; }
    public bool UserCanDelete { get; set; }
    public bool UserCanApprove { get; set; }
    public bool UserIsAdmin { get; set; }
    public bool UserCanViewCostSharePDFs { get; set; }
    public bool CanStartUpdate { get; set; }

    // Button visibility flags
    public bool CanViewFactSheet { get; set; }
    public bool HasExistingUpdateBatch { get; set; }
    public int? LatestUpdateBatchStateID { get; set; }
    public string? LatestUpdateBatchStateName { get; set; }
    public bool IsInLandownerAssistanceProgram { get; set; }
    public bool ExistsInImportBlockList { get; set; }
}
