//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[TreatmentUpdate]
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
        public static TreatmentUpdate GetTreatmentUpdate(this IQueryable<TreatmentUpdate> treatmentUpdates, int treatmentUpdateID)
        {
            var treatmentUpdate = treatmentUpdates.SingleOrDefault(x => x.TreatmentUpdateID == treatmentUpdateID);
            Check.RequireNotNullThrowNotFound(treatmentUpdate, "TreatmentUpdate", treatmentUpdateID);
            return treatmentUpdate;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteTreatmentUpdate(this IQueryable<TreatmentUpdate> treatmentUpdates, List<int> treatmentUpdateIDList)
        {
            if(treatmentUpdateIDList.Any())
            {
                var treatmentUpdatesInSourceCollectionToDelete = treatmentUpdates.Where(x => treatmentUpdateIDList.Contains(x.TreatmentUpdateID));
                foreach (var treatmentUpdateToDelete in treatmentUpdatesInSourceCollectionToDelete.ToList())
                {
                    treatmentUpdateToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteTreatmentUpdate(this IQueryable<TreatmentUpdate> treatmentUpdates, ICollection<TreatmentUpdate> treatmentUpdatesToDelete)
        {
            if(treatmentUpdatesToDelete.Any())
            {
                var treatmentUpdateIDList = treatmentUpdatesToDelete.Select(x => x.TreatmentUpdateID).ToList();
                var treatmentUpdatesToDeleteFromSourceList = treatmentUpdates.Where(x => treatmentUpdateIDList.Contains(x.TreatmentUpdateID)).ToList();

                foreach (var treatmentUpdateToDelete in treatmentUpdatesToDeleteFromSourceList)
                {
                    treatmentUpdateToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteTreatmentUpdate(this IQueryable<TreatmentUpdate> treatmentUpdates, int treatmentUpdateID)
        {
            DeleteTreatmentUpdate(treatmentUpdates, new List<int> { treatmentUpdateID });
        }

        public static void DeleteTreatmentUpdate(this IQueryable<TreatmentUpdate> treatmentUpdates, TreatmentUpdate treatmentUpdateToDelete)
        {
            DeleteTreatmentUpdate(treatmentUpdates, new List<TreatmentUpdate> { treatmentUpdateToDelete });
        }
    }
}