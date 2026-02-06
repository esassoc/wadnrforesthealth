namespace WADNR.Models.DataTransferObjects.ProjectUpdate;

/// <summary>
/// Summary of all diffs for a ProjectUpdateBatch, returned from API
/// </summary>
public class ProjectUpdateDiffSummary
{
    public string? BasicsDiffHtml { get; set; }
    public string? OrganizationsDiffHtml { get; set; }
    public string? ExternalLinksDiffHtml { get; set; }
    public string? NotesDiffHtml { get; set; }
    public string? ExpectedFundingDiffHtml { get; set; }

    public bool HasBasicsChanges { get; set; }
    public bool HasOrganizationsChanges { get; set; }
    public bool HasExternalLinksChanges { get; set; }
    public bool HasNotesChanges { get; set; }
    public bool HasExpectedFundingChanges { get; set; }

    public bool HasAnyChanges => HasBasicsChanges || HasOrganizationsChanges ||
                                  HasExternalLinksChanges || HasNotesChanges ||
                                  HasExpectedFundingChanges;
}
