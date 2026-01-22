namespace WADNR.Models.DataTransferObjects;

public class FundSourceNoteGridRow
{
    public int FundSourceNoteID { get; set; }
    public string Note { get; set; } = string.Empty;
    public string? CreatedByPersonName { get; set; }
    public DateTime CreateDate { get; set; }
    public string? UpdatedByPersonName { get; set; }
    public DateTime? UpdateDate { get; set; }
}

public class FundSourceNoteInternalGridRow
{
    public int FundSourceNoteInternalID { get; set; }
    public string Note { get; set; } = string.Empty;
    public string? CreatedByPersonName { get; set; }
    public DateTime CreateDate { get; set; }
    public string? UpdatedByPersonName { get; set; }
    public DateTime? UpdateDate { get; set; }
}
