namespace WADNR.Models.DataTransferObjects;

public class ProgramDetail
{
    public int ProgramID { get; set; }
    public string? ProgramName { get; set; }
    public string? ProgramShortName { get; set; }
    public bool ProgramIsActive { get; set; }
    public bool IsDefaultProgramForImportOnly { get; set; }
    public string? ProgramNotes { get; set; }

    // Parent Organization
    public int OrganizationID { get; set; }
    public string? OrganizationName { get; set; }

    // Primary Contact
    public int? PrimaryContactPersonID { get; set; }
    public string? PrimaryContactPersonFullName { get; set; }
    public string? PrimaryContactPersonOrganization { get; set; }

    // Program Document File
    public int? ProgramFileResourceID { get; set; }
    public string? ProgramFileResourceUrl { get; set; }
    public string? ProgramFileName { get; set; }

    // Program Example Geospatial File
    public int? ProgramExampleGeospatialUploadFileResourceID { get; set; }
    public string? ProgramExampleGeospatialUploadFileResourceUrl { get; set; }
    public string? ProgramExampleGeospatialUploadFileName { get; set; }

    // Program Editors (People)
    public List<PersonLookupItem> ProgramEditors { get; set; } = new();

    // Counts
    public int ProjectCount { get; set; }
}
