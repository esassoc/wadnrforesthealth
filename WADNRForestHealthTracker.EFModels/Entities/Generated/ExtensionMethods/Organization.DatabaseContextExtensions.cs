//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Organization]
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
        public static Organization GetOrganization(this IQueryable<Organization> organizations, int organizationID)
        {
            var organization = organizations.SingleOrDefault(x => x.OrganizationID == organizationID);
            Check.RequireNotNullThrowNotFound(organization, "Organization", organizationID);
            return organization;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteOrganization(this IQueryable<Organization> organizations, List<int> organizationIDList)
        {
            if(organizationIDList.Any())
            {
                var organizationsInSourceCollectionToDelete = organizations.Where(x => organizationIDList.Contains(x.OrganizationID));
                foreach (var organizationToDelete in organizationsInSourceCollectionToDelete.ToList())
                {
                    organizationToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteOrganization(this IQueryable<Organization> organizations, ICollection<Organization> organizationsToDelete)
        {
            if(organizationsToDelete.Any())
            {
                var organizationIDList = organizationsToDelete.Select(x => x.OrganizationID).ToList();
                var organizationsToDeleteFromSourceList = organizations.Where(x => organizationIDList.Contains(x.OrganizationID)).ToList();

                foreach (var organizationToDelete in organizationsToDeleteFromSourceList)
                {
                    organizationToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteOrganization(this IQueryable<Organization> organizations, int organizationID)
        {
            DeleteOrganization(organizations, new List<int> { organizationID });
        }

        public static void DeleteOrganization(this IQueryable<Organization> organizations, Organization organizationToDelete)
        {
            DeleteOrganization(organizations, new List<Organization> { organizationToDelete });
        }
    }
}