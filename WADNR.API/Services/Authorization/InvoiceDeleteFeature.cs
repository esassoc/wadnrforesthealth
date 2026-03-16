using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

/// <summary>
/// Restricts invoice/voucher deletion to Admin and EsaAdmin roles only.
/// Matches legacy InvoiceLineItemDeleteFeature which excluded ProjectSteward from delete operations.
/// </summary>
public class InvoiceDeleteFeature : BaseAuthorizationAttribute
{
    public InvoiceDeleteFeature() : base([RoleEnum.Admin, RoleEnum.EsaAdmin])
    {
    }
}
