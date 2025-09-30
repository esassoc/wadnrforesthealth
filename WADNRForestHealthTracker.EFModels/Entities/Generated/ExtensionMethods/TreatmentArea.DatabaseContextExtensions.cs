//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[TreatmentArea]
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
        public static TreatmentArea GetTreatmentArea(this IQueryable<TreatmentArea> treatmentAreas, int treatmentAreaID)
        {
            var treatmentArea = treatmentAreas.SingleOrDefault(x => x.TreatmentAreaID == treatmentAreaID);
            Check.RequireNotNullThrowNotFound(treatmentArea, "TreatmentArea", treatmentAreaID);
            return treatmentArea;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteTreatmentArea(this IQueryable<TreatmentArea> treatmentAreas, List<int> treatmentAreaIDList)
        {
            if(treatmentAreaIDList.Any())
            {
                var treatmentAreasInSourceCollectionToDelete = treatmentAreas.Where(x => treatmentAreaIDList.Contains(x.TreatmentAreaID));
                foreach (var treatmentAreaToDelete in treatmentAreasInSourceCollectionToDelete.ToList())
                {
                    treatmentAreaToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteTreatmentArea(this IQueryable<TreatmentArea> treatmentAreas, ICollection<TreatmentArea> treatmentAreasToDelete)
        {
            if(treatmentAreasToDelete.Any())
            {
                var treatmentAreaIDList = treatmentAreasToDelete.Select(x => x.TreatmentAreaID).ToList();
                var treatmentAreasToDeleteFromSourceList = treatmentAreas.Where(x => treatmentAreaIDList.Contains(x.TreatmentAreaID)).ToList();

                foreach (var treatmentAreaToDelete in treatmentAreasToDeleteFromSourceList)
                {
                    treatmentAreaToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteTreatmentArea(this IQueryable<TreatmentArea> treatmentAreas, int treatmentAreaID)
        {
            DeleteTreatmentArea(treatmentAreas, new List<int> { treatmentAreaID });
        }

        public static void DeleteTreatmentArea(this IQueryable<TreatmentArea> treatmentAreas, TreatmentArea treatmentAreaToDelete)
        {
            DeleteTreatmentArea(treatmentAreas, new List<TreatmentArea> { treatmentAreaToDelete });
        }
    }
}