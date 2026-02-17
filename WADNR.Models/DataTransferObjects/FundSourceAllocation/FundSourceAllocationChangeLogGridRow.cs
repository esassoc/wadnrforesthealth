namespace WADNR.Models.DataTransferObjects.FundSourceAllocation;

public class FundSourceAllocationChangeLogGridRow
{
    public int FundSourceAllocationChangeLogID { get; set; }
    public decimal? OldValue { get; set; }
    public decimal? NewValue { get; set; }
    public string? Note { get; set; }
    public int ChangePersonID { get; set; }
    public string ChangePersonName { get; set; } = string.Empty;
    public DateTime ChangeDate { get; set; }
}
