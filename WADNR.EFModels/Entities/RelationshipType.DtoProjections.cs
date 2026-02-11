using System.Linq;
using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class RelationshipTypeProjections
{
    public static readonly Expression<Func<RelationshipType, RelationshipTypeGridRow>> AsGridRow = x => new RelationshipTypeGridRow
    {
        RelationshipTypeID = x.RelationshipTypeID,
        RelationshipTypeName = x.RelationshipTypeName,
        CanStewardProjects = x.CanStewardProjects,
        IsPrimaryContact = x.IsPrimaryContact,
        CanOnlyBeRelatedOnceToAProject = x.CanOnlyBeRelatedOnceToAProject,
        ShowOnFactSheet = x.ShowOnFactSheet,
        ReportInAccomplishmentsDashboard = x.ReportInAccomplishmentsDashboard,
        RelationshipTypeDescription = x.RelationshipTypeDescription,
        AssociatedOrganizationTypeNames = x.OrganizationTypeRelationshipTypes
            .Select(otrt => otrt.OrganizationType.OrganizationTypeName)
            .ToList(),
        ProjectOrganizationCount = x.ProjectOrganizations.Count,
    };

    public static readonly Expression<Func<RelationshipType, RelationshipTypeLookupItem>> AsLookupItem = x => new RelationshipTypeLookupItem
    {
        RelationshipTypeID = x.RelationshipTypeID,
        RelationshipTypeName = x.RelationshipTypeName,
    };

    public static readonly Expression<Func<RelationshipType, RelationshipTypeSummary>> AsSummary = x => new RelationshipTypeSummary
    {
        RelationshipTypeID = x.RelationshipTypeID,
        RelationshipTypeName = x.RelationshipTypeName,
        RelationshipTypeDescription = x.RelationshipTypeDescription,
        CanStewardProjects = x.CanStewardProjects,
        IsPrimaryContact = x.IsPrimaryContact,
        CanOnlyBeRelatedOnceToAProject = x.CanOnlyBeRelatedOnceToAProject,
    };
}
