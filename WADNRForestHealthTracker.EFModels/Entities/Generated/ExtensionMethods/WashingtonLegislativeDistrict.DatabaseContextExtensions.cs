//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WashingtonLegislativeDistrict]
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
        public static WashingtonLegislativeDistrict GetWashingtonLegislativeDistrict(this IQueryable<WashingtonLegislativeDistrict> washingtonLegislativeDistricts, int washingtonLegislativeDistrictID)
        {
            var washingtonLegislativeDistrict = washingtonLegislativeDistricts.SingleOrDefault(x => x.WashingtonLegislativeDistrictID == washingtonLegislativeDistrictID);
            Check.RequireNotNullThrowNotFound(washingtonLegislativeDistrict, "WashingtonLegislativeDistrict", washingtonLegislativeDistrictID);
            return washingtonLegislativeDistrict;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteWashingtonLegislativeDistrict(this IQueryable<WashingtonLegislativeDistrict> washingtonLegislativeDistricts, List<int> washingtonLegislativeDistrictIDList)
        {
            if(washingtonLegislativeDistrictIDList.Any())
            {
                var washingtonLegislativeDistrictsInSourceCollectionToDelete = washingtonLegislativeDistricts.Where(x => washingtonLegislativeDistrictIDList.Contains(x.WashingtonLegislativeDistrictID));
                foreach (var washingtonLegislativeDistrictToDelete in washingtonLegislativeDistrictsInSourceCollectionToDelete.ToList())
                {
                    washingtonLegislativeDistrictToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteWashingtonLegislativeDistrict(this IQueryable<WashingtonLegislativeDistrict> washingtonLegislativeDistricts, ICollection<WashingtonLegislativeDistrict> washingtonLegislativeDistrictsToDelete)
        {
            if(washingtonLegislativeDistrictsToDelete.Any())
            {
                var washingtonLegislativeDistrictIDList = washingtonLegislativeDistrictsToDelete.Select(x => x.WashingtonLegislativeDistrictID).ToList();
                var washingtonLegislativeDistrictsToDeleteFromSourceList = washingtonLegislativeDistricts.Where(x => washingtonLegislativeDistrictIDList.Contains(x.WashingtonLegislativeDistrictID)).ToList();

                foreach (var washingtonLegislativeDistrictToDelete in washingtonLegislativeDistrictsToDeleteFromSourceList)
                {
                    washingtonLegislativeDistrictToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteWashingtonLegislativeDistrict(this IQueryable<WashingtonLegislativeDistrict> washingtonLegislativeDistricts, int washingtonLegislativeDistrictID)
        {
            DeleteWashingtonLegislativeDistrict(washingtonLegislativeDistricts, new List<int> { washingtonLegislativeDistrictID });
        }

        public static void DeleteWashingtonLegislativeDistrict(this IQueryable<WashingtonLegislativeDistrict> washingtonLegislativeDistricts, WashingtonLegislativeDistrict washingtonLegislativeDistrictToDelete)
        {
            DeleteWashingtonLegislativeDistrict(washingtonLegislativeDistricts, new List<WashingtonLegislativeDistrict> { washingtonLegislativeDistrictToDelete });
        }
    }
}