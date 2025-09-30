//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[County]
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
        public static County GetCounty(this IQueryable<County> counties, int countyID)
        {
            var county = counties.SingleOrDefault(x => x.CountyID == countyID);
            Check.RequireNotNullThrowNotFound(county, "County", countyID);
            return county;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteCounty(this IQueryable<County> counties, List<int> countyIDList)
        {
            if(countyIDList.Any())
            {
                var countiesInSourceCollectionToDelete = counties.Where(x => countyIDList.Contains(x.CountyID));
                foreach (var countyToDelete in countiesInSourceCollectionToDelete.ToList())
                {
                    countyToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteCounty(this IQueryable<County> counties, ICollection<County> countiesToDelete)
        {
            if(countiesToDelete.Any())
            {
                var countyIDList = countiesToDelete.Select(x => x.CountyID).ToList();
                var countiesToDeleteFromSourceList = counties.Where(x => countyIDList.Contains(x.CountyID)).ToList();

                foreach (var countyToDelete in countiesToDeleteFromSourceList)
                {
                    countyToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteCounty(this IQueryable<County> counties, int countyID)
        {
            DeleteCounty(counties, new List<int> { countyID });
        }

        public static void DeleteCounty(this IQueryable<County> counties, County countyToDelete)
        {
            DeleteCounty(counties, new List<County> { countyToDelete });
        }
    }
}