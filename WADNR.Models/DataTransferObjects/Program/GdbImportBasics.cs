namespace WADNR.Models.DataTransferObjects;

public class GdbImportBasics
{
    public int GisUploadSourceOrganizationID { get; set; }
    public string? ProjectTypeDefaultName { get; set; }
    public string? TreatmentTypeDefaultName { get; set; }
    public bool? ImportIsFlattened { get; set; }
    public bool AdjustProjectTypeBasedOnTreatmentTypes { get; set; }
    public int ProjectStageDefaultID { get; set; }
    public string? ProjectStageDefaultName { get; set; }
    public bool DataDeriveProjectStage { get; set; }
    public string? DefaultLeadImplementerOrganizationName { get; set; }
    public bool ImportAsDetailedLocationInsteadOfTreatments { get; set; }
    public bool ImportAsDetailedLocationInAdditionToTreatments { get; set; }
    public string? ProjectDescriptionDefaultText { get; set; }
    public bool ApplyStartDateToProject { get; set; }
    public bool ApplyCompletedDateToProject { get; set; }
    public bool ApplyStartDateToTreatments { get; set; }
    public bool ApplyEndDateToTreatments { get; set; }
}
