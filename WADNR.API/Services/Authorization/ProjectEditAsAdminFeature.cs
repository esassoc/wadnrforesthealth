using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows admin-level direct editing of project data (notes, photos, documents).
/// Normal users must use the project update workflow instead.
/// Includes ProjectSteward, Admin, EsaAdmin, and CanEditProgram roles.
/// </summary>
public class ProjectEditAsAdminFeature : BaseAuthorizationAttribute
{
    public ProjectEditAsAdminFeature() : base([
        RoleEnum.ProjectSteward,
        RoleEnum.Admin,
        RoleEnum.EsaAdmin,
        RoleEnum.CanEditProgram
    ])
    {
    }
}
