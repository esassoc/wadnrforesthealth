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
    public bool HasLocationData { get; set; }
    public string? ProjectLocationNotes { get; set; }
    public List<string> Counties { get; set; } = new();
    public List<string> Regions { get; set; } = new();
    public List<string> PriorityLandscapes { get; set; } = new();

    // User permission flags (populated based on calling user's role)
    public bool UserCanEdit { get; set; }
    public bool UserCanDirectEdit { get; set; }
    public bool UserCanDelete { get; set; }
    public bool UserCanApprove { get; set; }
    public bool UserIsAdmin { get; set; }

    // Button visibility flags
    public bool CanViewFactSheet { get; set; }
    public bool CanStartUpdate { get; set; }
    public bool HasExistingUpdateBatch { get; set; }
    public int? LatestUpdateBatchStateID { get; set; }
    public string? LatestUpdateBatchStateName { get; set; }
    public bool IsInLandownerAssistanceProgram { get; set; }
    public bool ExistsInImportBlockList { get; set; }
}

public class ProjectOrganizationItem
{
    public int ProjectOrganizationID { get; set; }
    public int OrganizationID { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public int RelationshipTypeID { get; set; }
    public string RelationshipTypeName { get; set; } = string.Empty;
    public bool IsPrimaryContact { get; set; }
}

public class ProjectPersonItem
{
    public int ProjectPersonID { get; set; }
    public int PersonID { get; set; }
    public string PersonFullName { get; set; } = string.Empty;
    public int RelationshipTypeID { get; set; }
    public string RelationshipTypeName { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

public class TagLookupItem
{
    public int TagID { get; set; }
    public string TagName { get; set; } = string.Empty;
}

public class FundSourceAllocationRequestItem
{
    public int ProjectFundSourceAllocationRequestID { get; set; }
    public int FundSourceAllocationID { get; set; }
    public string FundSourceAllocationName { get; set; } = string.Empty;
    public string FundSourceName { get; set; } = string.Empty;
    public decimal? MatchAmount { get; set; }
    public decimal? PayAmount { get; set; }
    public decimal? TotalAmount { get; set; }
}

public class AgreementLookupItem
{
    public int AgreementID { get; set; }
    public string AgreementTitle { get; set; } = string.Empty;
    public string? AgreementNumber { get; set; }
}
