//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GisDefaultMapping]
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
        public static GisDefaultMapping GetGisDefaultMapping(this IQueryable<GisDefaultMapping> gisDefaultMappings, int gisDefaultMappingID)
        {
            var gisDefaultMapping = gisDefaultMappings.SingleOrDefault(x => x.GisDefaultMappingID == gisDefaultMappingID);
            Check.RequireNotNullThrowNotFound(gisDefaultMapping, "GisDefaultMapping", gisDefaultMappingID);
            return gisDefaultMapping;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteGisDefaultMapping(this IQueryable<GisDefaultMapping> gisDefaultMappings, List<int> gisDefaultMappingIDList)
        {
            if(gisDefaultMappingIDList.Any())
            {
                var gisDefaultMappingsInSourceCollectionToDelete = gisDefaultMappings.Where(x => gisDefaultMappingIDList.Contains(x.GisDefaultMappingID));
                foreach (var gisDefaultMappingToDelete in gisDefaultMappingsInSourceCollectionToDelete.ToList())
                {
                    gisDefaultMappingToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteGisDefaultMapping(this IQueryable<GisDefaultMapping> gisDefaultMappings, ICollection<GisDefaultMapping> gisDefaultMappingsToDelete)
        {
            if(gisDefaultMappingsToDelete.Any())
            {
                var gisDefaultMappingIDList = gisDefaultMappingsToDelete.Select(x => x.GisDefaultMappingID).ToList();
                var gisDefaultMappingsToDeleteFromSourceList = gisDefaultMappings.Where(x => gisDefaultMappingIDList.Contains(x.GisDefaultMappingID)).ToList();

                foreach (var gisDefaultMappingToDelete in gisDefaultMappingsToDeleteFromSourceList)
                {
                    gisDefaultMappingToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteGisDefaultMapping(this IQueryable<GisDefaultMapping> gisDefaultMappings, int gisDefaultMappingID)
        {
            DeleteGisDefaultMapping(gisDefaultMappings, new List<int> { gisDefaultMappingID });
        }

        public static void DeleteGisDefaultMapping(this IQueryable<GisDefaultMapping> gisDefaultMappings, GisDefaultMapping gisDefaultMappingToDelete)
        {
            DeleteGisDefaultMapping(gisDefaultMappings, new List<GisDefaultMapping> { gisDefaultMappingToDelete });
        }
    }
}