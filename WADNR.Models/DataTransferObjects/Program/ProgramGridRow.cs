namespace WADNR.Models.DataTransferObjects;

public class ProgramGridRow
{
    public int ProgramID { get; set; }
    public string ProgramName { get; set; }

    public string ProgramShortName { get; set; }
    public bool IsActive { get; set; }
    public bool IsDefaultProgramForImportOnly { get; set; }
    public OrganizationLookupItem? Organization { get; set; }
    public int ProjectCount { get; set; }

}