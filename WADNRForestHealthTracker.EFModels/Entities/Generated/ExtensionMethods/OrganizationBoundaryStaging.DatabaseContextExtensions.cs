//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[OrganizationBoundaryStaging]
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
        public static OrganizationBoundaryStaging GetOrganizationBoundaryStaging(this IQueryable<OrganizationBoundaryStaging> organizationBoundaryStagings, int organizationBoundaryStagingID)
        {
            var organizationBoundaryStaging = organizationBoundaryStagings.SingleOrDefault(x => x.OrganizationBoundaryStagingID == organizationBoundaryStagingID);
            Check.RequireNotNullThrowNotFound(organizationBoundaryStaging, "OrganizationBoundaryStaging", organizationBoundaryStagingID);
            return organizationBoundaryStaging;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteOrganizationBoundaryStaging(this IQueryable<OrganizationBoundaryStaging> organizationBoundaryStagings, List<int> organizationBoundaryStagingIDList)
        {
            if(organizationBoundaryStagingIDList.Any())
            {
                var organizationBoundaryStagingsInSourceCollectionToDelete = organizationBoundaryStagings.Where(x => organizationBoundaryStagingIDList.Contains(x.OrganizationBoundaryStagingID));
                foreach (var organizationBoundaryStagingToDelete in organizationBoundaryStagingsInSourceCollectionToDelete.ToList())
                {
                    organizationBoundaryStagingToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteOrganizationBoundaryStaging(this IQueryable<OrganizationBoundaryStaging> organizationBoundaryStagings, ICollection<OrganizationBoundaryStaging> organizationBoundaryStagingsToDelete)
        {
            if(organizationBoundaryStagingsToDelete.Any())
            {
                var organizationBoundaryStagingIDList = organizationBoundaryStagingsToDelete.Select(x => x.OrganizationBoundaryStagingID).ToList();
                var organizationBoundaryStagingsToDeleteFromSourceList = organizationBoundaryStagings.Where(x => organizationBoundaryStagingIDList.Contains(x.OrganizationBoundaryStagingID)).ToList();

                foreach (var organizationBoundaryStagingToDelete in organizationBoundaryStagingsToDeleteFromSourceList)
                {
                    organizationBoundaryStagingToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteOrganizationBoundaryStaging(this IQueryable<OrganizationBoundaryStaging> organizationBoundaryStagings, int organizationBoundaryStagingID)
        {
            DeleteOrganizationBoundaryStaging(organizationBoundaryStagings, new List<int> { organizationBoundaryStagingID });
        }

        public static void DeleteOrganizationBoundaryStaging(this IQueryable<OrganizationBoundaryStaging> organizationBoundaryStagings, OrganizationBoundaryStaging organizationBoundaryStagingToDelete)
        {
            DeleteOrganizationBoundaryStaging(organizationBoundaryStagings, new List<OrganizationBoundaryStaging> { organizationBoundaryStagingToDelete });
        }
    }
}