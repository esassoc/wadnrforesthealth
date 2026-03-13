namespace WADNR.Models.DataTransferObjects;

public class ProjectNoteExcelRow
{
    public int ProjectID { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public string? CreatedByPersonName { get; set; }
    public DateTimeOffset? CreateDate { get; set; }
}
