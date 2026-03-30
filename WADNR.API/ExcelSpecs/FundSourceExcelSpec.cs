using WADNR.Common.ExcelWorkbookUtilities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.ExcelSpecs;

public class FundSourceExcelSpec : ExcelWorksheetSpec<FundSourceExcelRow>
{
    public FundSourceExcelSpec()
    {
        AddColumn("Fund Source Number", x => x.FundSourceNumber ?? string.Empty);
        AddColumn("CFDA #", x => x.CFDANumber ?? string.Empty);
        AddColumn("Fund Source Name", x => x.FundSourceName);
        AddColumn("Total Award Amount", x => x.TotalAwardAmount, "$#,##0.00");
        AddColumn("Fund Source Start Date", x => x.StartDate, "mm/dd/yyyy");
        AddColumn("Fund Source End Date", x => x.EndDate, "mm/dd/yyyy");
        AddColumn("Fund Source Status", x => x.FundSourceStatusName);
        AddColumn("Fund Source Type", x => x.FundSourceTypeDisplay ?? string.Empty);
    }
}
