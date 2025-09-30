//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GisUploadAttempt]
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
        public static GisUploadAttempt GetGisUploadAttempt(this IQueryable<GisUploadAttempt> gisUploadAttempts, int gisUploadAttemptID)
        {
            var gisUploadAttempt = gisUploadAttempts.SingleOrDefault(x => x.GisUploadAttemptID == gisUploadAttemptID);
            Check.RequireNotNullThrowNotFound(gisUploadAttempt, "GisUploadAttempt", gisUploadAttemptID);
            return gisUploadAttempt;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteGisUploadAttempt(this IQueryable<GisUploadAttempt> gisUploadAttempts, List<int> gisUploadAttemptIDList)
        {
            if(gisUploadAttemptIDList.Any())
            {
                var gisUploadAttemptsInSourceCollectionToDelete = gisUploadAttempts.Where(x => gisUploadAttemptIDList.Contains(x.GisUploadAttemptID));
                foreach (var gisUploadAttemptToDelete in gisUploadAttemptsInSourceCollectionToDelete.ToList())
                {
                    gisUploadAttemptToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteGisUploadAttempt(this IQueryable<GisUploadAttempt> gisUploadAttempts, ICollection<GisUploadAttempt> gisUploadAttemptsToDelete)
        {
            if(gisUploadAttemptsToDelete.Any())
            {
                var gisUploadAttemptIDList = gisUploadAttemptsToDelete.Select(x => x.GisUploadAttemptID).ToList();
                var gisUploadAttemptsToDeleteFromSourceList = gisUploadAttempts.Where(x => gisUploadAttemptIDList.Contains(x.GisUploadAttemptID)).ToList();

                foreach (var gisUploadAttemptToDelete in gisUploadAttemptsToDeleteFromSourceList)
                {
                    gisUploadAttemptToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteGisUploadAttempt(this IQueryable<GisUploadAttempt> gisUploadAttempts, int gisUploadAttemptID)
        {
            DeleteGisUploadAttempt(gisUploadAttempts, new List<int> { gisUploadAttemptID });
        }

        public static void DeleteGisUploadAttempt(this IQueryable<GisUploadAttempt> gisUploadAttempts, GisUploadAttempt gisUploadAttemptToDelete)
        {
            DeleteGisUploadAttempt(gisUploadAttempts, new List<GisUploadAttempt> { gisUploadAttemptToDelete });
        }
    }
}