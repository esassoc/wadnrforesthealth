namespace WADNR.Models.DataTransferObjects;

public class ProjectInternalNoteGridRow
{
    public int ProjectInternalNoteID { get; set; }
    public string Note { get; set; } = string.Empty;
    public string? CreatedByPersonName { get; set; }
    public DateTime CreateDate { get; set; }
    public string? UpdatedByPersonName { get; set; }
    public DateTime? UpdateDate { get; set; }
}
