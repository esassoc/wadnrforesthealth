namespace WADNR.Models.DataTransferObjects;

/// <summary>
/// A funding source option for checkboxes (Federal, State, Private, Other).
/// </summary>
public class FundingSourceOption
{
    public int FundingSourceID { get; set; }
    public string FundingSourceName { get; set; } = string.Empty;
}
