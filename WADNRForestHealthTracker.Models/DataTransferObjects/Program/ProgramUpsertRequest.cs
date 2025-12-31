namespace WADNRForestHealthTracker.Models.DataTransferObjects
{
    public class ProgramUpsertRequest
    {
        // Core identity
        public string? ProgramName { get; set; } = string.Empty;
        public string? ProgramShortName { get; set; }

        // Relationships
        public int OrganizationID { get; set; }
        public int? ProgramPrimaryContactPersonID { get; set; }

        // Flags / metadata
        public bool ProgramIsActive { get; set; }
        public bool IsDefaultProgramForImportOnly { get; set; }

        // Files / notes
        public string? ProgramNotes { get; set; }
        public int? ProgramFileResourceID { get; set; }
        public int? ProgramExampleGeospatialUploadFileResourceID { get; set; }
    }
}
