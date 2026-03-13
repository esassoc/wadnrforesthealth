using WADNR.Common.ExcelWorkbookUtilities;
using WADNR.Models.DataTransferObjects.FundSourceAllocation;

namespace WADNR.API.ExcelSpecs;

public class FundSourceAllocationExcelSpec : ExcelWorksheetSpec<FundSourceAllocationExcelRow>
{
    public FundSourceAllocationExcelSpec()
    {
        AddColumn("Fund Source Number", x => x.FundSourceNumber);
        AddColumn("Fund Source Allocation Name", x => x.FundSourceAllocationName ?? string.Empty);
        AddColumn("Program Manager", x => x.ProgramManagerNames ?? string.Empty);
        AddColumn("Fund Source Start Date", x => x.StartDate, "mm/dd/yyyy");
        AddColumn("Fund Source End Date", x => x.EndDate, "mm/dd/yyyy");
        AddColumn("Parent FundSource Fund Source Status", x => x.ParentFundSourceStatusName ?? string.Empty);
        AddColumn("DNR Upland Region", x => x.DNRUplandRegionName ?? string.Empty);
        AddColumn("Federal Fund Code", x => x.FederalFundCodeDisplay ?? string.Empty);
        AddColumn("Allocation Amount", x => x.AllocationAmount, "$#,##0.00");
        AddColumn("Program Index Project Code", x => x.ProgramIndexProjectCodeDisplay ?? string.Empty);
        AddColumn("Organization", x => x.OrganizationName ?? string.Empty);
    }
}
