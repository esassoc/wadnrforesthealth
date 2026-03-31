using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Allows access to users who can create, edit, and delete invoices.
/// Includes ProjectSteward, Admin, and EsaAdmin roles.
/// Matches legacy InvoiceCreateFeature / InvoiceEditFeature.
/// </summary>
public class InvoiceManageFeature : BaseAuthorizationAttribute
{
    public InvoiceManageFeature() : base([
        RoleEnum.ProjectSteward,
        RoleEnum.Admin,
        RoleEnum.EsaAdmin
    ])
    {
    }
}
