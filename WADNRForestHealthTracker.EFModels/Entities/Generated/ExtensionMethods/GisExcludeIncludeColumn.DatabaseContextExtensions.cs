//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GisExcludeIncludeColumn]
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
        public static GisExcludeIncludeColumn GetGisExcludeIncludeColumn(this IQueryable<GisExcludeIncludeColumn> gisExcludeIncludeColumns, int gisExcludeIncludeColumnID)
        {
            var gisExcludeIncludeColumn = gisExcludeIncludeColumns.SingleOrDefault(x => x.GisExcludeIncludeColumnID == gisExcludeIncludeColumnID);
            Check.RequireNotNullThrowNotFound(gisExcludeIncludeColumn, "GisExcludeIncludeColumn", gisExcludeIncludeColumnID);
            return gisExcludeIncludeColumn;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteGisExcludeIncludeColumn(this IQueryable<GisExcludeIncludeColumn> gisExcludeIncludeColumns, List<int> gisExcludeIncludeColumnIDList)
        {
            if(gisExcludeIncludeColumnIDList.Any())
            {
                var gisExcludeIncludeColumnsInSourceCollectionToDelete = gisExcludeIncludeColumns.Where(x => gisExcludeIncludeColumnIDList.Contains(x.GisExcludeIncludeColumnID));
                foreach (var gisExcludeIncludeColumnToDelete in gisExcludeIncludeColumnsInSourceCollectionToDelete.ToList())
                {
                    gisExcludeIncludeColumnToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteGisExcludeIncludeColumn(this IQueryable<GisExcludeIncludeColumn> gisExcludeIncludeColumns, ICollection<GisExcludeIncludeColumn> gisExcludeIncludeColumnsToDelete)
        {
            if(gisExcludeIncludeColumnsToDelete.Any())
            {
                var gisExcludeIncludeColumnIDList = gisExcludeIncludeColumnsToDelete.Select(x => x.GisExcludeIncludeColumnID).ToList();
                var gisExcludeIncludeColumnsToDeleteFromSourceList = gisExcludeIncludeColumns.Where(x => gisExcludeIncludeColumnIDList.Contains(x.GisExcludeIncludeColumnID)).ToList();

                foreach (var gisExcludeIncludeColumnToDelete in gisExcludeIncludeColumnsToDeleteFromSourceList)
                {
                    gisExcludeIncludeColumnToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteGisExcludeIncludeColumn(this IQueryable<GisExcludeIncludeColumn> gisExcludeIncludeColumns, int gisExcludeIncludeColumnID)
        {
            DeleteGisExcludeIncludeColumn(gisExcludeIncludeColumns, new List<int> { gisExcludeIncludeColumnID });
        }

        public static void DeleteGisExcludeIncludeColumn(this IQueryable<GisExcludeIncludeColumn> gisExcludeIncludeColumns, GisExcludeIncludeColumn gisExcludeIncludeColumnToDelete)
        {
            DeleteGisExcludeIncludeColumn(gisExcludeIncludeColumns, new List<GisExcludeIncludeColumn> { gisExcludeIncludeColumnToDelete });
        }
    }
}