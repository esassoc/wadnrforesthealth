using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows access to Excel download endpoints.
/// Requires Admin, EsaAdmin, or ProjectSteward role.
/// </summary>
public class ExcelDownloadFeature : BaseAuthorizationAttribute
{
    public ExcelDownloadFeature() : base([RoleEnum.Admin, RoleEnum.EsaAdmin, RoleEnum.ProjectSteward])
    {
    }
}
