using System.Linq;
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

    public static readonly Expression<Func<OrganizationType, OrganizationTypeGridRow>> AsGridRow = x => new OrganizationTypeGridRow
    {
        OrganizationTypeID = x.OrganizationTypeID,
        OrganizationTypeName = x.OrganizationTypeName,
        OrganizationTypeAbbreviation = x.OrganizationTypeAbbreviation,
        LegendColor = x.LegendColor,
        ShowOnProjectMaps = x.ShowOnProjectMaps,
        IsDefaultOrganizationType = x.IsDefaultOrganizationType,
        IsFundingType = x.IsFundingType,
        OrganizationCount = x.Organizations.Count,
    };
}
