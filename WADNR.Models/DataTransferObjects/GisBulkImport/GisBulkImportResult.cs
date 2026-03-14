namespace WADNR.Models.DataTransferObjects.GisBulkImport;

public class GisBulkImportResult
{
    public int ProjectsCreated { get; set; }
    public int ProjectsUpdated { get; set; }
    public int ProjectsSkipped { get; set; }
    public int TreatmentsCreated { get; set; }
    public int LocationsCreated { get; set; }
    public List<GisBulkImportProjectResult> CreatedProjects { get; set; } = new();
    public List<GisBulkImportProjectResult> UpdatedProjects { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

public class GisBulkImportProjectResult
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; }
}
