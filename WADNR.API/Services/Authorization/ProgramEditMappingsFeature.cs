using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows access to users who can edit GIS import configuration and download project GDBs.
/// Includes Admin, EsaAdmin, and CanEditProgram supplemental role.
/// Matches legacy ProgramEditMappingsFeature / ProgramEditorFeature.
/// </summary>
public class ProgramEditMappingsFeature : BaseAuthorizationAttribute
{
    public ProgramEditMappingsFeature() : base([
        RoleEnum.Admin,
        RoleEnum.EsaAdmin,
        RoleEnum.CanEditProgram
    ])
    {
    }
}
