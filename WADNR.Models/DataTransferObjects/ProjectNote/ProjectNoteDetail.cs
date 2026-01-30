namespace WADNR.Models.DataTransferObjects;

public class ProjectNoteDetail
{
    public int ProjectNoteID { get; set; }
    public int ProjectID { get; set; }
    public string Note { get; set; } = string.Empty;
    public string? CreatedByPersonName { get; set; }
    public DateTime CreateDate { get; set; }
    public string? UpdatedByPersonName { get; set; }
    public DateTime? UpdateDate { get; set; }
}
