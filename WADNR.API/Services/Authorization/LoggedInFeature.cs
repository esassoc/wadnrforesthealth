using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows access to any authenticated user regardless of role.
/// Use for actions that require login but not specific permissions.
/// </summary>
public class LoggedInFeature : BaseAuthorizationAttribute
{
    public LoggedInFeature() : base([])
    {
    }
}
