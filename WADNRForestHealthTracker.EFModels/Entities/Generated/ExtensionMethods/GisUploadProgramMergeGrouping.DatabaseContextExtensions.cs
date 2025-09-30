//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GisUploadProgramMergeGrouping]
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
        public static GisUploadProgramMergeGrouping GetGisUploadProgramMergeGrouping(this IQueryable<GisUploadProgramMergeGrouping> gisUploadProgramMergeGroupings, int gisUploadProgramMergeGroupingID)
        {
            var gisUploadProgramMergeGrouping = gisUploadProgramMergeGroupings.SingleOrDefault(x => x.GisUploadProgramMergeGroupingID == gisUploadProgramMergeGroupingID);
            Check.RequireNotNullThrowNotFound(gisUploadProgramMergeGrouping, "GisUploadProgramMergeGrouping", gisUploadProgramMergeGroupingID);
            return gisUploadProgramMergeGrouping;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteGisUploadProgramMergeGrouping(this IQueryable<GisUploadProgramMergeGrouping> gisUploadProgramMergeGroupings, List<int> gisUploadProgramMergeGroupingIDList)
        {
            if(gisUploadProgramMergeGroupingIDList.Any())
            {
                var gisUploadProgramMergeGroupingsInSourceCollectionToDelete = gisUploadProgramMergeGroupings.Where(x => gisUploadProgramMergeGroupingIDList.Contains(x.GisUploadProgramMergeGroupingID));
                foreach (var gisUploadProgramMergeGroupingToDelete in gisUploadProgramMergeGroupingsInSourceCollectionToDelete.ToList())
                {
                    gisUploadProgramMergeGroupingToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteGisUploadProgramMergeGrouping(this IQueryable<GisUploadProgramMergeGrouping> gisUploadProgramMergeGroupings, ICollection<GisUploadProgramMergeGrouping> gisUploadProgramMergeGroupingsToDelete)
        {
            if(gisUploadProgramMergeGroupingsToDelete.Any())
            {
                var gisUploadProgramMergeGroupingIDList = gisUploadProgramMergeGroupingsToDelete.Select(x => x.GisUploadProgramMergeGroupingID).ToList();
                var gisUploadProgramMergeGroupingsToDeleteFromSourceList = gisUploadProgramMergeGroupings.Where(x => gisUploadProgramMergeGroupingIDList.Contains(x.GisUploadProgramMergeGroupingID)).ToList();

                foreach (var gisUploadProgramMergeGroupingToDelete in gisUploadProgramMergeGroupingsToDeleteFromSourceList)
                {
                    gisUploadProgramMergeGroupingToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteGisUploadProgramMergeGrouping(this IQueryable<GisUploadProgramMergeGrouping> gisUploadProgramMergeGroupings, int gisUploadProgramMergeGroupingID)
        {
            DeleteGisUploadProgramMergeGrouping(gisUploadProgramMergeGroupings, new List<int> { gisUploadProgramMergeGroupingID });
        }

        public static void DeleteGisUploadProgramMergeGrouping(this IQueryable<GisUploadProgramMergeGrouping> gisUploadProgramMergeGroupings, GisUploadProgramMergeGrouping gisUploadProgramMergeGroupingToDelete)
        {
            DeleteGisUploadProgramMergeGrouping(gisUploadProgramMergeGroupings, new List<GisUploadProgramMergeGrouping> { gisUploadProgramMergeGroupingToDelete });
        }
    }
}