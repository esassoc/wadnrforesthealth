//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[StateProvince]
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
        public static StateProvince GetStateProvince(this IQueryable<StateProvince> stateProvinces, int stateProvinceID)
        {
            var stateProvince = stateProvinces.SingleOrDefault(x => x.StateProvinceID == stateProvinceID);
            Check.RequireNotNullThrowNotFound(stateProvince, "StateProvince", stateProvinceID);
            return stateProvince;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteStateProvince(this IQueryable<StateProvince> stateProvinces, List<int> stateProvinceIDList)
        {
            if(stateProvinceIDList.Any())
            {
                var stateProvincesInSourceCollectionToDelete = stateProvinces.Where(x => stateProvinceIDList.Contains(x.StateProvinceID));
                foreach (var stateProvinceToDelete in stateProvincesInSourceCollectionToDelete.ToList())
                {
                    stateProvinceToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteStateProvince(this IQueryable<StateProvince> stateProvinces, ICollection<StateProvince> stateProvincesToDelete)
        {
            if(stateProvincesToDelete.Any())
            {
                var stateProvinceIDList = stateProvincesToDelete.Select(x => x.StateProvinceID).ToList();
                var stateProvincesToDeleteFromSourceList = stateProvinces.Where(x => stateProvinceIDList.Contains(x.StateProvinceID)).ToList();

                foreach (var stateProvinceToDelete in stateProvincesToDeleteFromSourceList)
                {
                    stateProvinceToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteStateProvince(this IQueryable<StateProvince> stateProvinces, int stateProvinceID)
        {
            DeleteStateProvince(stateProvinces, new List<int> { stateProvinceID });
        }

        public static void DeleteStateProvince(this IQueryable<StateProvince> stateProvinces, StateProvince stateProvinceToDelete)
        {
            DeleteStateProvince(stateProvinces, new List<StateProvince> { stateProvinceToDelete });
        }
    }
}