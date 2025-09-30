//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GisExcludeIncludeColumnValue]
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
        public static GisExcludeIncludeColumnValue GetGisExcludeIncludeColumnValue(this IQueryable<GisExcludeIncludeColumnValue> gisExcludeIncludeColumnValues, int gisExcludeIncludeColumnValueID)
        {
            var gisExcludeIncludeColumnValue = gisExcludeIncludeColumnValues.SingleOrDefault(x => x.GisExcludeIncludeColumnValueID == gisExcludeIncludeColumnValueID);
            Check.RequireNotNullThrowNotFound(gisExcludeIncludeColumnValue, "GisExcludeIncludeColumnValue", gisExcludeIncludeColumnValueID);
            return gisExcludeIncludeColumnValue;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteGisExcludeIncludeColumnValue(this IQueryable<GisExcludeIncludeColumnValue> gisExcludeIncludeColumnValues, List<int> gisExcludeIncludeColumnValueIDList)
        {
            if(gisExcludeIncludeColumnValueIDList.Any())
            {
                var gisExcludeIncludeColumnValuesInSourceCollectionToDelete = gisExcludeIncludeColumnValues.Where(x => gisExcludeIncludeColumnValueIDList.Contains(x.GisExcludeIncludeColumnValueID));
                foreach (var gisExcludeIncludeColumnValueToDelete in gisExcludeIncludeColumnValuesInSourceCollectionToDelete.ToList())
                {
                    gisExcludeIncludeColumnValueToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteGisExcludeIncludeColumnValue(this IQueryable<GisExcludeIncludeColumnValue> gisExcludeIncludeColumnValues, ICollection<GisExcludeIncludeColumnValue> gisExcludeIncludeColumnValuesToDelete)
        {
            if(gisExcludeIncludeColumnValuesToDelete.Any())
            {
                var gisExcludeIncludeColumnValueIDList = gisExcludeIncludeColumnValuesToDelete.Select(x => x.GisExcludeIncludeColumnValueID).ToList();
                var gisExcludeIncludeColumnValuesToDeleteFromSourceList = gisExcludeIncludeColumnValues.Where(x => gisExcludeIncludeColumnValueIDList.Contains(x.GisExcludeIncludeColumnValueID)).ToList();

                foreach (var gisExcludeIncludeColumnValueToDelete in gisExcludeIncludeColumnValuesToDeleteFromSourceList)
                {
                    gisExcludeIncludeColumnValueToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteGisExcludeIncludeColumnValue(this IQueryable<GisExcludeIncludeColumnValue> gisExcludeIncludeColumnValues, int gisExcludeIncludeColumnValueID)
        {
            DeleteGisExcludeIncludeColumnValue(gisExcludeIncludeColumnValues, new List<int> { gisExcludeIncludeColumnValueID });
        }

        public static void DeleteGisExcludeIncludeColumnValue(this IQueryable<GisExcludeIncludeColumnValue> gisExcludeIncludeColumnValues, GisExcludeIncludeColumnValue gisExcludeIncludeColumnValueToDelete)
        {
            DeleteGisExcludeIncludeColumnValue(gisExcludeIncludeColumnValues, new List<GisExcludeIncludeColumnValue> { gisExcludeIncludeColumnValueToDelete });
        }
    }
}