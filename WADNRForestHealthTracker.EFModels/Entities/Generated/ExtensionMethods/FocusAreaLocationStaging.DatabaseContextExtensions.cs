//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FocusAreaLocationStaging]
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
        public static FocusAreaLocationStaging GetFocusAreaLocationStaging(this IQueryable<FocusAreaLocationStaging> focusAreaLocationStagings, int focusAreaLocationStagingID)
        {
            var focusAreaLocationStaging = focusAreaLocationStagings.SingleOrDefault(x => x.FocusAreaLocationStagingID == focusAreaLocationStagingID);
            Check.RequireNotNullThrowNotFound(focusAreaLocationStaging, "FocusAreaLocationStaging", focusAreaLocationStagingID);
            return focusAreaLocationStaging;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteFocusAreaLocationStaging(this IQueryable<FocusAreaLocationStaging> focusAreaLocationStagings, List<int> focusAreaLocationStagingIDList)
        {
            if(focusAreaLocationStagingIDList.Any())
            {
                var focusAreaLocationStagingsInSourceCollectionToDelete = focusAreaLocationStagings.Where(x => focusAreaLocationStagingIDList.Contains(x.FocusAreaLocationStagingID));
                foreach (var focusAreaLocationStagingToDelete in focusAreaLocationStagingsInSourceCollectionToDelete.ToList())
                {
                    focusAreaLocationStagingToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteFocusAreaLocationStaging(this IQueryable<FocusAreaLocationStaging> focusAreaLocationStagings, ICollection<FocusAreaLocationStaging> focusAreaLocationStagingsToDelete)
        {
            if(focusAreaLocationStagingsToDelete.Any())
            {
                var focusAreaLocationStagingIDList = focusAreaLocationStagingsToDelete.Select(x => x.FocusAreaLocationStagingID).ToList();
                var focusAreaLocationStagingsToDeleteFromSourceList = focusAreaLocationStagings.Where(x => focusAreaLocationStagingIDList.Contains(x.FocusAreaLocationStagingID)).ToList();

                foreach (var focusAreaLocationStagingToDelete in focusAreaLocationStagingsToDeleteFromSourceList)
                {
                    focusAreaLocationStagingToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteFocusAreaLocationStaging(this IQueryable<FocusAreaLocationStaging> focusAreaLocationStagings, int focusAreaLocationStagingID)
        {
            DeleteFocusAreaLocationStaging(focusAreaLocationStagings, new List<int> { focusAreaLocationStagingID });
        }

        public static void DeleteFocusAreaLocationStaging(this IQueryable<FocusAreaLocationStaging> focusAreaLocationStagings, FocusAreaLocationStaging focusAreaLocationStagingToDelete)
        {
            DeleteFocusAreaLocationStaging(focusAreaLocationStagings, new List<FocusAreaLocationStaging> { focusAreaLocationStagingToDelete });
        }
    }
}