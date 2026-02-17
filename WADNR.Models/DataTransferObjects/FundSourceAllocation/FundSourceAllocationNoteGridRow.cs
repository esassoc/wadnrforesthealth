namespace WADNR.Models.DataTransferObjects.FundSourceAllocation;

public class FundSourceAllocationNoteGridRow
{
    public int FundSourceAllocationNoteID { get; set; }
    public int FundSourceAllocationID { get; set; }
    public string? Note { get; set; }
    public string? CreatedByPersonName { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? UpdatedByPersonName { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
