//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FocusArea]
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
        public static FocusArea GetFocusArea(this IQueryable<FocusArea> focusAreas, int focusAreaID)
        {
            var focusArea = focusAreas.SingleOrDefault(x => x.FocusAreaID == focusAreaID);
            Check.RequireNotNullThrowNotFound(focusArea, "FocusArea", focusAreaID);
            return focusArea;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteFocusArea(this IQueryable<FocusArea> focusAreas, List<int> focusAreaIDList)
        {
            if(focusAreaIDList.Any())
            {
                var focusAreasInSourceCollectionToDelete = focusAreas.Where(x => focusAreaIDList.Contains(x.FocusAreaID));
                foreach (var focusAreaToDelete in focusAreasInSourceCollectionToDelete.ToList())
                {
                    focusAreaToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteFocusArea(this IQueryable<FocusArea> focusAreas, ICollection<FocusArea> focusAreasToDelete)
        {
            if(focusAreasToDelete.Any())
            {
                var focusAreaIDList = focusAreasToDelete.Select(x => x.FocusAreaID).ToList();
                var focusAreasToDeleteFromSourceList = focusAreas.Where(x => focusAreaIDList.Contains(x.FocusAreaID)).ToList();

                foreach (var focusAreaToDelete in focusAreasToDeleteFromSourceList)
                {
                    focusAreaToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteFocusArea(this IQueryable<FocusArea> focusAreas, int focusAreaID)
        {
            DeleteFocusArea(focusAreas, new List<int> { focusAreaID });
        }

        public static void DeleteFocusArea(this IQueryable<FocusArea> focusAreas, FocusArea focusAreaToDelete)
        {
            DeleteFocusArea(focusAreas, new List<FocusArea> { focusAreaToDelete });
        }
    }
}