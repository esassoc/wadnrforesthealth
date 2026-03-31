namespace WADNR.Models.DataTransferObjects.GisBulkImport;

public class GisUploadSourceOrganizationSummary
{
    public int GisUploadSourceOrganizationID { get; set; }
    public string GisUploadSourceOrganizationName { get; set; } = string.Empty;
    public string ProgramName { get; set; } = string.Empty;
    public string ProgramDisplayName { get; set; } = string.Empty;
    public int ProgramID { get; set; }
}
