namespace WADNR.Models.DataTransferObjects;

public class ProjectBasicsEditData
{
    // Per-field import flags
    public bool IsProjectNameImported { get; set; }
    public bool IsProjectStageImported { get; set; }
    public bool IsProjectInitiationDateImported { get; set; }
    public bool IsCompletionDateImported { get; set; }
    public bool IsProjectIdentifierImported { get; set; }
    public string ImportedFieldWarningMessage { get; set; } = "This field is imported for the program. To edit data, visit the system of record.";
}
