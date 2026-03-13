using WADNR.Common.ExcelWorkbookUtilities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.ExcelSpecs;

public class ProjectClassificationExcelSpec : ExcelWorksheetSpec<ProjectClassificationExcelRow>
{
    public ProjectClassificationExcelSpec()
    {
        AddColumn("Project ID", x => x.ProjectID);
        AddColumn("Project Name", x => x.ProjectName);
        AddColumn("Classification", x => x.ClassificationName);
    }
}
