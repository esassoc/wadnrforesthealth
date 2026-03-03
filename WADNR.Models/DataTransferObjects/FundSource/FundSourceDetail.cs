namespace WADNR.Models.DataTransferObjects;

public class FundSourceDetail
{
    public int FundSourceID { get; set; }
    public string FundSourceName { get; set; } = string.Empty;
    public string? FundSourceNumber { get; set; }
    public string? ShortName { get; set; }
    public string FundSourceTitle { get; set; } = string.Empty;

    // Organization
    public OrganizationLookupItem Organization { get; set; } = new();

    // Status
    public FundSourceStatusLookupItem? FundSourceStatus { get; set; }

    // Type
    public int? FundSourceTypeID { get; set; }
    public string? FundSourceTypeName { get; set; }

    // Financial
    public decimal TotalAwardAmount { get; set; }
    public decimal? CurrentBalance { get; set; }
    public string? CFDANumber { get; set; }

    // Dates
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // Additional details
    public string? ConditionsAndRequirements { get; set; }
    public string? ComplianceNotes { get; set; }

    // Counts for related entities
    public int AllocationCount { get; set; }
    public int AgreementCount { get; set; }
    public int ProjectCount { get; set; }
    public int FileCount { get; set; }
    public int NoteCount { get; set; }
    public int InternalNoteCount { get; set; }
}

public class FundSourceStatusLookupItem
{
    public int FundSourceStatusID { get; set; }
    public string FundSourceStatusName { get; set; } = string.Empty;
}
