using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class OrganizationTypeProjections
{
    public static Expression<Func<OrganizationType, OrganizationTypeLookupItem>> AsLookupItem => x => new OrganizationTypeLookupItem
    {
        OrganizationTypeID = x.OrganizationTypeID,
        OrganizationTypeName = x.OrganizationTypeName
    };
}
