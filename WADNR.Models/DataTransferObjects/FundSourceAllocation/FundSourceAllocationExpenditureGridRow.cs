namespace WADNR.Models.DataTransferObjects.FundSourceAllocation;

public class FundSourceAllocationExpenditureGridRow
{
    public int FundSourceAllocationExpenditureID { get; set; }
    public int? CostTypeID { get; set; }
    public string? CostTypeName { get; set; }
    public int Biennium { get; set; }
    public int FiscalMonth { get; set; }
    public int CalendarYear { get; set; }
    public int CalendarMonth { get; set; }
    public decimal ExpenditureAmount { get; set; }
}
