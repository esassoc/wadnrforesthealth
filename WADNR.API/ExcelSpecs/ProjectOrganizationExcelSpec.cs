using WADNR.Common.ExcelWorkbookUtilities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.ExcelSpecs;

public class ProjectOrganizationExcelSpec : ExcelWorksheetSpec<ProjectOrganizationExcelRow>
{
    public ProjectOrganizationExcelSpec()
    {
        AddColumn("Project ID", x => x.ProjectID);
        AddColumn("Project Name", x => x.ProjectName);
        AddColumn("Contributing Organization ID", x => x.OrganizationID);
        AddColumn("Contributing Organization Name", x => x.OrganizationName);
        AddColumn("Contributing Organization Primary Contact for Contributing Organization", x => x.PrimaryContactPersonName ?? string.Empty);
        AddColumn("Contributing Organization Type", x => x.OrganizationTypeName ?? string.Empty);
        AddColumn("Contributing Organization Relationship To Project", x => x.RelationshipTypeName);
    }
}
