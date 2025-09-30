//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GisUploadSourceOrganization]
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
        public static GisUploadSourceOrganization GetGisUploadSourceOrganization(this IQueryable<GisUploadSourceOrganization> gisUploadSourceOrganizations, int gisUploadSourceOrganizationID)
        {
            var gisUploadSourceOrganization = gisUploadSourceOrganizations.SingleOrDefault(x => x.GisUploadSourceOrganizationID == gisUploadSourceOrganizationID);
            Check.RequireNotNullThrowNotFound(gisUploadSourceOrganization, "GisUploadSourceOrganization", gisUploadSourceOrganizationID);
            return gisUploadSourceOrganization;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteGisUploadSourceOrganization(this IQueryable<GisUploadSourceOrganization> gisUploadSourceOrganizations, List<int> gisUploadSourceOrganizationIDList)
        {
            if(gisUploadSourceOrganizationIDList.Any())
            {
                var gisUploadSourceOrganizationsInSourceCollectionToDelete = gisUploadSourceOrganizations.Where(x => gisUploadSourceOrganizationIDList.Contains(x.GisUploadSourceOrganizationID));
                foreach (var gisUploadSourceOrganizationToDelete in gisUploadSourceOrganizationsInSourceCollectionToDelete.ToList())
                {
                    gisUploadSourceOrganizationToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteGisUploadSourceOrganization(this IQueryable<GisUploadSourceOrganization> gisUploadSourceOrganizations, ICollection<GisUploadSourceOrganization> gisUploadSourceOrganizationsToDelete)
        {
            if(gisUploadSourceOrganizationsToDelete.Any())
            {
                var gisUploadSourceOrganizationIDList = gisUploadSourceOrganizationsToDelete.Select(x => x.GisUploadSourceOrganizationID).ToList();
                var gisUploadSourceOrganizationsToDeleteFromSourceList = gisUploadSourceOrganizations.Where(x => gisUploadSourceOrganizationIDList.Contains(x.GisUploadSourceOrganizationID)).ToList();

                foreach (var gisUploadSourceOrganizationToDelete in gisUploadSourceOrganizationsToDeleteFromSourceList)
                {
                    gisUploadSourceOrganizationToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteGisUploadSourceOrganization(this IQueryable<GisUploadSourceOrganization> gisUploadSourceOrganizations, int gisUploadSourceOrganizationID)
        {
            DeleteGisUploadSourceOrganization(gisUploadSourceOrganizations, new List<int> { gisUploadSourceOrganizationID });
        }

        public static void DeleteGisUploadSourceOrganization(this IQueryable<GisUploadSourceOrganization> gisUploadSourceOrganizations, GisUploadSourceOrganization gisUploadSourceOrganizationToDelete)
        {
            DeleteGisUploadSourceOrganization(gisUploadSourceOrganizations, new List<GisUploadSourceOrganization> { gisUploadSourceOrganizationToDelete });
        }
    }
}