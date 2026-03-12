using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

/// <summary>
/// Static methods for program-scoped authorization checks.
/// </summary>
public static class ProgramAuthorization
{
    /// <summary>
    /// Checks whether a CanEditProgram user can manage a specific program.
    /// The person's AssignedPrograms must include the given programID.
    /// </summary>
    public static bool CanEditProgram(PersonDetail person, int programID)
    {
        // Admin/EsaAdmin bypass all scoping
        if (person.BaseRole?.RoleID is (int)RoleEnum.Admin or (int)RoleEnum.EsaAdmin)
            return true;

        if (!person.HasCanEditProgramRole())
            return false;

        return person.AssignedPrograms.Any(p => p.ProgramID == programID);
    }
}
