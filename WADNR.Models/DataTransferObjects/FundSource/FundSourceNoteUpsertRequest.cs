namespace WADNR.Models.DataTransferObjects;

public class FundSourceNoteUpsertRequest
{
    public int FundSourceID { get; set; }
    public string Note { get; set; } = string.Empty;
}
