//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ForesterWorkUnit]
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
        public static ForesterWorkUnit GetForesterWorkUnit(this IQueryable<ForesterWorkUnit> foresterWorkUnits, int foresterWorkUnitID)
        {
            var foresterWorkUnit = foresterWorkUnits.SingleOrDefault(x => x.ForesterWorkUnitID == foresterWorkUnitID);
            Check.RequireNotNullThrowNotFound(foresterWorkUnit, "ForesterWorkUnit", foresterWorkUnitID);
            return foresterWorkUnit;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteForesterWorkUnit(this IQueryable<ForesterWorkUnit> foresterWorkUnits, List<int> foresterWorkUnitIDList)
        {
            if(foresterWorkUnitIDList.Any())
            {
                var foresterWorkUnitsInSourceCollectionToDelete = foresterWorkUnits.Where(x => foresterWorkUnitIDList.Contains(x.ForesterWorkUnitID));
                foreach (var foresterWorkUnitToDelete in foresterWorkUnitsInSourceCollectionToDelete.ToList())
                {
                    foresterWorkUnitToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteForesterWorkUnit(this IQueryable<ForesterWorkUnit> foresterWorkUnits, ICollection<ForesterWorkUnit> foresterWorkUnitsToDelete)
        {
            if(foresterWorkUnitsToDelete.Any())
            {
                var foresterWorkUnitIDList = foresterWorkUnitsToDelete.Select(x => x.ForesterWorkUnitID).ToList();
                var foresterWorkUnitsToDeleteFromSourceList = foresterWorkUnits.Where(x => foresterWorkUnitIDList.Contains(x.ForesterWorkUnitID)).ToList();

                foreach (var foresterWorkUnitToDelete in foresterWorkUnitsToDeleteFromSourceList)
                {
                    foresterWorkUnitToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteForesterWorkUnit(this IQueryable<ForesterWorkUnit> foresterWorkUnits, int foresterWorkUnitID)
        {
            DeleteForesterWorkUnit(foresterWorkUnits, new List<int> { foresterWorkUnitID });
        }

        public static void DeleteForesterWorkUnit(this IQueryable<ForesterWorkUnit> foresterWorkUnits, ForesterWorkUnit foresterWorkUnitToDelete)
        {
            DeleteForesterWorkUnit(foresterWorkUnits, new List<ForesterWorkUnit> { foresterWorkUnitToDelete });
        }
    }
}