using WADNR.Common.ExcelWorkbookUtilities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.ExcelSpecs;

public class ProjectExcelSpec : ExcelWorksheetSpec<ProjectExcelRow>
{
    public ProjectExcelSpec()
    {
        AddColumn("Project Name", x => x.ProjectName);
        AddColumn("FHT Project Number", x => x.FhtProjectNumber);
        AddColumn("Program Identifier", x => x.ProjectGisIdentifier ?? string.Empty);
        AddColumn("Program", x => x.ProgramNames);
        AddColumn("Non-Lead Implementing Contributing Organizations", x => x.NonLeadImplementingOrganizations);
        AddColumn("Project Stage", x => x.ProjectStageName);
        AddColumn("Project Themes", x => x.ProjectThemes);
        AddColumn("Priority Landscapes", x => x.PriorityLandscapeNames);
        AddColumn("DNR Upland Region", x => x.DNRUplandRegionNames);
        AddColumn("County", x => x.CountyNames);
        AddColumn("DNR LOA Focus Area", x => x.FocusAreaName ?? string.Empty);
        AddColumn("Project Initiation date", x => x.PlannedDate, "mm/dd/yyyy");
        AddColumn("Completion Date", x => x.CompletionDate, "mm/dd/yyyy");
        AddColumn("Project Description", x => x.ProjectDescription ?? string.Empty);
        AddColumn("Estimated Total Cost", x => x.EstimatedTotalCost, "$#,##0.00");
        AddColumn("Total Amount", x => x.TotalFundingAmount, "$#,##0.00");
        AddColumn("Project Location Notes", x => x.ProjectLocationNotes ?? string.Empty);
    }
}
