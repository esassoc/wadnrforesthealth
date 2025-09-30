//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[OrganizationCode]
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
        public static OrganizationCode GetOrganizationCode(this IQueryable<OrganizationCode> organizationCodes, int organizationCodeID)
        {
            var organizationCode = organizationCodes.SingleOrDefault(x => x.OrganizationCodeID == organizationCodeID);
            Check.RequireNotNullThrowNotFound(organizationCode, "OrganizationCode", organizationCodeID);
            return organizationCode;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteOrganizationCode(this IQueryable<OrganizationCode> organizationCodes, List<int> organizationCodeIDList)
        {
            if(organizationCodeIDList.Any())
            {
                var organizationCodesInSourceCollectionToDelete = organizationCodes.Where(x => organizationCodeIDList.Contains(x.OrganizationCodeID));
                foreach (var organizationCodeToDelete in organizationCodesInSourceCollectionToDelete.ToList())
                {
                    organizationCodeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteOrganizationCode(this IQueryable<OrganizationCode> organizationCodes, ICollection<OrganizationCode> organizationCodesToDelete)
        {
            if(organizationCodesToDelete.Any())
            {
                var organizationCodeIDList = organizationCodesToDelete.Select(x => x.OrganizationCodeID).ToList();
                var organizationCodesToDeleteFromSourceList = organizationCodes.Where(x => organizationCodeIDList.Contains(x.OrganizationCodeID)).ToList();

                foreach (var organizationCodeToDelete in organizationCodesToDeleteFromSourceList)
                {
                    organizationCodeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteOrganizationCode(this IQueryable<OrganizationCode> organizationCodes, int organizationCodeID)
        {
            DeleteOrganizationCode(organizationCodes, new List<int> { organizationCodeID });
        }

        public static void DeleteOrganizationCode(this IQueryable<OrganizationCode> organizationCodes, OrganizationCode organizationCodeToDelete)
        {
            DeleteOrganizationCode(organizationCodes, new List<OrganizationCode> { organizationCodeToDelete });
        }
    }
}