using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Restricts access to GIS bulk import operations.
/// Matches legacy GisUploadAttemptCreateFeature: Admin, EsaAdmin, ProjectSteward.
/// </summary>
public class GisBulkImportFeature : BaseAuthorizationAttribute
{
    public GisBulkImportFeature() : base([RoleEnum.Admin, RoleEnum.EsaAdmin, RoleEnum.ProjectSteward])
    {
    }
}
