namespace WADNR.Models.DataTransferObjects.GisBulkImport;

public class GisBulkImportResult
{
    public int ProjectsCreated { get; set; }
    public int ProjectsUpdated { get; set; }
    public int ProjectsSkipped { get; set; }
    public int TreatmentsCreated { get; set; }
    public int LocationsCreated { get; set; }
    public List<string> Warnings { get; set; } = new();
}
