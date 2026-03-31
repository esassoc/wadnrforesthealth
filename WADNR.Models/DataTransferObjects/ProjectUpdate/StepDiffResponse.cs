namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// Response for a single step's diff check in the Project Update workflow.
/// Contains structured sections instead of raw HTML.
/// </summary>
public class StepDiffResponse
{
    public bool HasChanges { get; set; }
    public List<DiffSection> Sections { get; set; } = [];
}

/// <summary>
/// A section within a step diff (e.g., "Documents", "Fund Source Allocations").
/// The Type determines which fields are populated.
/// </summary>
public class DiffSection
{
    /// <summary>Optional heading for the section (e.g., "Documents", "Funding Sources").</summary>
    public string? Title { get; set; }

    /// <summary>"fields" | "table" | "list"</summary>
    public string Type { get; set; } = "fields";

    /// <summary>For "fields" type: label/value pairs comparing original vs updated.</summary>
    public List<DiffField>? Fields { get; set; }

    /// <summary>For "table" type: column headers.</summary>
    public List<string>? Headers { get; set; }

    /// <summary>For "table" type: rows from the original (approved) project.</summary>
    public List<List<string>>? OriginalRows { get; set; }

    /// <summary>For "table" type: rows from the updated project.</summary>
    public List<List<string>>? UpdatedRows { get; set; }

    /// <summary>For "list" type: items from the original (approved) project.</summary>
    public List<string>? OriginalItems { get; set; }

    /// <summary>For "list" type: items from the updated project.</summary>
    public List<string>? UpdatedItems { get; set; }
}

/// <summary>
/// A single field comparison within a "fields" section.
/// </summary>
public class DiffField
{
    public string Label { get; set; } = "";
    public string OriginalValue { get; set; } = "";
    public string UpdatedValue { get; set; } = "";
}
