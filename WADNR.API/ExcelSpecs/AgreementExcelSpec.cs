using WADNR.Common.ExcelWorkbookUtilities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.ExcelSpecs;

public class AgreementExcelSpec : ExcelWorksheetSpec<AgreementExcelRow>
{
    public AgreementExcelSpec()
    {
        AddColumn("Type", x => x.AgreementTypeAbbrev ?? string.Empty);
        AddColumn("Number", x => x.AgreementNumber ?? string.Empty);
        AddColumn("WA DNR Fund Source Allocation", x => x.FundSourceAllocationNumbers);
        AddColumn("Contributing Organization", x => x.OrganizationName);
        AddColumn("Title", x => x.AgreementTitle);
        AddColumn("Start Date", x => x.StartDate, "mm/dd/yyyy");
        AddColumn("End Date", x => x.EndDate, "mm/dd/yyyy");
        AddColumn("Amount", x => x.AgreementAmount, "$#,##0.00");
    }
}
