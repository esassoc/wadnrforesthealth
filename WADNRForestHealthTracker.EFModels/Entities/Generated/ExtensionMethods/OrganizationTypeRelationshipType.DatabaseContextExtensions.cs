//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[OrganizationTypeRelationshipType]
using System.Collections.Generic;
using System.Linq;
using Z.EntityFramework.Plus;
using CodeFirstStoreFunctions;
using LtInfo.Common.DesignByContract;
using LtInfo.Common.Models;
using ProjectFirma.Web.Common;

namespace ProjectFirma.Web.Models
{
    public static partial class DatabaseContextExtensions
    {
        public static OrganizationTypeRelationshipType GetOrganizationTypeRelationshipType(this IQueryable<OrganizationTypeRelationshipType> organizationTypeRelationshipTypes, int organizationTypeRelationshipTypeID)
        {
            var organizationTypeRelationshipType = organizationTypeRelationshipTypes.SingleOrDefault(x => x.OrganizationTypeRelationshipTypeID == organizationTypeRelationshipTypeID);
            Check.RequireNotNullThrowNotFound(organizationTypeRelationshipType, "OrganizationTypeRelationshipType", organizationTypeRelationshipTypeID);
            return organizationTypeRelationshipType;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteOrganizationTypeRelationshipType(this IQueryable<OrganizationTypeRelationshipType> organizationTypeRelationshipTypes, List<int> organizationTypeRelationshipTypeIDList)
        {
            if(organizationTypeRelationshipTypeIDList.Any())
            {
                var organizationTypeRelationshipTypesInSourceCollectionToDelete = organizationTypeRelationshipTypes.Where(x => organizationTypeRelationshipTypeIDList.Contains(x.OrganizationTypeRelationshipTypeID));
                foreach (var organizationTypeRelationshipTypeToDelete in organizationTypeRelationshipTypesInSourceCollectionToDelete.ToList())
                {
                    organizationTypeRelationshipTypeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteOrganizationTypeRelationshipType(this IQueryable<OrganizationTypeRelationshipType> organizationTypeRelationshipTypes, ICollection<OrganizationTypeRelationshipType> organizationTypeRelationshipTypesToDelete)
        {
            if(organizationTypeRelationshipTypesToDelete.Any())
            {
                var organizationTypeRelationshipTypeIDList = organizationTypeRelationshipTypesToDelete.Select(x => x.OrganizationTypeRelationshipTypeID).ToList();
                var organizationTypeRelationshipTypesToDeleteFromSourceList = organizationTypeRelationshipTypes.Where(x => organizationTypeRelationshipTypeIDList.Contains(x.OrganizationTypeRelationshipTypeID)).ToList();

                foreach (var organizationTypeRelationshipTypeToDelete in organizationTypeRelationshipTypesToDeleteFromSourceList)
                {
                    organizationTypeRelationshipTypeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteOrganizationTypeRelationshipType(this IQueryable<OrganizationTypeRelationshipType> organizationTypeRelationshipTypes, int organizationTypeRelationshipTypeID)
        {
            DeleteOrganizationTypeRelationshipType(organizationTypeRelationshipTypes, new List<int> { organizationTypeRelationshipTypeID });
        }

        public static void DeleteOrganizationTypeRelationshipType(this IQueryable<OrganizationTypeRelationshipType> organizationTypeRelationshipTypes, OrganizationTypeRelationshipType organizationTypeRelationshipTypeToDelete)
        {
            DeleteOrganizationTypeRelationshipType(organizationTypeRelationshipTypes, new List<OrganizationTypeRelationshipType> { organizationTypeRelationshipTypeToDelete });
        }
    }
}