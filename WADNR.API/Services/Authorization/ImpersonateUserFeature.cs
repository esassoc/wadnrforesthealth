using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

public class ImpersonateUserFeature : BaseAuthorizationAttribute
{
    public ImpersonateUserFeature() : base([RoleEnum.Admin, RoleEnum.EsaAdmin])
    {
    }
}
