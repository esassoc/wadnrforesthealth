namespace WADNR.Models.DataTransferObjects;

public class DNRUplandRegionExpenditureByCostTypeRow
{
    public string CostTypeName { get; set; } = string.Empty;
    public int CalendarYear { get; set; }
    public decimal ExpenditureAmount { get; set; }
}
