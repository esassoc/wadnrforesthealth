//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[DNRUplandRegion]
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
        public static DNRUplandRegion GetDNRUplandRegion(this IQueryable<DNRUplandRegion> dNRUplandRegions, int dNRUplandRegionID)
        {
            var dNRUplandRegion = dNRUplandRegions.SingleOrDefault(x => x.DNRUplandRegionID == dNRUplandRegionID);
            Check.RequireNotNullThrowNotFound(dNRUplandRegion, "DNRUplandRegion", dNRUplandRegionID);
            return dNRUplandRegion;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteDNRUplandRegion(this IQueryable<DNRUplandRegion> dNRUplandRegions, List<int> dNRUplandRegionIDList)
        {
            if(dNRUplandRegionIDList.Any())
            {
                var dNRUplandRegionsInSourceCollectionToDelete = dNRUplandRegions.Where(x => dNRUplandRegionIDList.Contains(x.DNRUplandRegionID));
                foreach (var dNRUplandRegionToDelete in dNRUplandRegionsInSourceCollectionToDelete.ToList())
                {
                    dNRUplandRegionToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteDNRUplandRegion(this IQueryable<DNRUplandRegion> dNRUplandRegions, ICollection<DNRUplandRegion> dNRUplandRegionsToDelete)
        {
            if(dNRUplandRegionsToDelete.Any())
            {
                var dNRUplandRegionIDList = dNRUplandRegionsToDelete.Select(x => x.DNRUplandRegionID).ToList();
                var dNRUplandRegionsToDeleteFromSourceList = dNRUplandRegions.Where(x => dNRUplandRegionIDList.Contains(x.DNRUplandRegionID)).ToList();

                foreach (var dNRUplandRegionToDelete in dNRUplandRegionsToDeleteFromSourceList)
                {
                    dNRUplandRegionToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteDNRUplandRegion(this IQueryable<DNRUplandRegion> dNRUplandRegions, int dNRUplandRegionID)
        {
            DeleteDNRUplandRegion(dNRUplandRegions, new List<int> { dNRUplandRegionID });
        }

        public static void DeleteDNRUplandRegion(this IQueryable<DNRUplandRegion> dNRUplandRegions, DNRUplandRegion dNRUplandRegionToDelete)
        {
            DeleteDNRUplandRegion(dNRUplandRegions, new List<DNRUplandRegion> { dNRUplandRegionToDelete });
        }
    }
}