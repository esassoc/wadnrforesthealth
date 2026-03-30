using WADNR.Common.ExcelWorkbookUtilities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.ExcelSpecs;

public class ProjectDescriptionExcelSpec : ExcelWorksheetSpec<ProjectDescriptionExcelRow>
{
    public ProjectDescriptionExcelSpec()
    {
        AddColumn("Project ID", x => x.ProjectID);
        AddColumn("Project Name", x => x.ProjectName);
        AddColumn("Description", x => x.ProjectDescription ?? string.Empty);
    }
}
