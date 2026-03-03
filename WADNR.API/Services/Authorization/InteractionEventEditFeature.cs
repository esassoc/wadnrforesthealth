using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows access to users who can edit interaction events.
/// Includes ProjectSteward, Admin, and EsaAdmin.
/// </summary>
public class InteractionEventEditFeature : BaseAuthorizationAttribute
{
    public InteractionEventEditFeature() : base([
        RoleEnum.ProjectSteward,
        RoleEnum.Admin,
        RoleEnum.EsaAdmin,
    ])
    {
    }
}
