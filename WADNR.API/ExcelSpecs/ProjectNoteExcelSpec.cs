using WADNR.Common.ExcelWorkbookUtilities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.ExcelSpecs;

public class ProjectNoteExcelSpec : ExcelWorksheetSpec<ProjectNoteExcelRow>
{
    public ProjectNoteExcelSpec()
    {
        AddColumn("Project ID", x => x.ProjectID);
        AddColumn("Project Name", x => x.ProjectName);
        AddColumn("Project Note", x => x.Note);
        AddColumn("Create Person", x => x.CreatedByPersonName ?? string.Empty);
        AddColumn("Create Date", x => x.CreateDate, "mm/dd/yyyy");
    }
}
