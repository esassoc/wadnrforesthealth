//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[OrganizationType]
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
        public static OrganizationType GetOrganizationType(this IQueryable<OrganizationType> organizationTypes, int organizationTypeID)
        {
            var organizationType = organizationTypes.SingleOrDefault(x => x.OrganizationTypeID == organizationTypeID);
            Check.RequireNotNullThrowNotFound(organizationType, "OrganizationType", organizationTypeID);
            return organizationType;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteOrganizationType(this IQueryable<OrganizationType> organizationTypes, List<int> organizationTypeIDList)
        {
            if(organizationTypeIDList.Any())
            {
                var organizationTypesInSourceCollectionToDelete = organizationTypes.Where(x => organizationTypeIDList.Contains(x.OrganizationTypeID));
                foreach (var organizationTypeToDelete in organizationTypesInSourceCollectionToDelete.ToList())
                {
                    organizationTypeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteOrganizationType(this IQueryable<OrganizationType> organizationTypes, ICollection<OrganizationType> organizationTypesToDelete)
        {
            if(organizationTypesToDelete.Any())
            {
                var organizationTypeIDList = organizationTypesToDelete.Select(x => x.OrganizationTypeID).ToList();
                var organizationTypesToDeleteFromSourceList = organizationTypes.Where(x => organizationTypeIDList.Contains(x.OrganizationTypeID)).ToList();

                foreach (var organizationTypeToDelete in organizationTypesToDeleteFromSourceList)
                {
                    organizationTypeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteOrganizationType(this IQueryable<OrganizationType> organizationTypes, int organizationTypeID)
        {
            DeleteOrganizationType(organizationTypes, new List<int> { organizationTypeID });
        }

        public static void DeleteOrganizationType(this IQueryable<OrganizationType> organizationTypes, OrganizationType organizationTypeToDelete)
        {
            DeleteOrganizationType(organizationTypes, new List<OrganizationType> { organizationTypeToDelete });
        }
    }
}