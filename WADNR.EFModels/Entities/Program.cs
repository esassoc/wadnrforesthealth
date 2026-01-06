namespace WADNR.EFModels.Entities;

public partial class Program
{
    public string InternalDisplayName =>
        $"{ProgramName}{(!string.IsNullOrWhiteSpace(ProgramShortName) ? $" ({ProgramShortName})" : string.Empty)}{(!ProgramIsActive ? " (Inactive)" : string.Empty)}";

    public string DisplayName =>
        IsDefaultProgramForImportOnly
            ? Organization.DisplayName
            : InternalDisplayName;
}