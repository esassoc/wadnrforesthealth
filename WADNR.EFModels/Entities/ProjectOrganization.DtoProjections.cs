using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ProjectOrganizationProjections
{
    public static Expression<Func<ProjectOrganization, ProjectOrganizationItem>> AsItem => x => new ProjectOrganizationItem
    {
        ProjectOrganizationID = x.ProjectOrganizationID,
        OrganizationID = x.OrganizationID,
        OrganizationName = x.Organization.OrganizationName,
        RelationshipTypeID = x.RelationshipTypeID,
        RelationshipTypeName = x.RelationshipType.RelationshipTypeName,
        IsPrimaryContact = x.RelationshipType.IsPrimaryContact
    };
}
