//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Treatment]
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
        public static Treatment GetTreatment(this IQueryable<Treatment> treatments, int treatmentID)
        {
            var treatment = treatments.SingleOrDefault(x => x.TreatmentID == treatmentID);
            Check.RequireNotNullThrowNotFound(treatment, "Treatment", treatmentID);
            return treatment;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteTreatment(this IQueryable<Treatment> treatments, List<int> treatmentIDList)
        {
            if(treatmentIDList.Any())
            {
                var treatmentsInSourceCollectionToDelete = treatments.Where(x => treatmentIDList.Contains(x.TreatmentID));
                foreach (var treatmentToDelete in treatmentsInSourceCollectionToDelete.ToList())
                {
                    treatmentToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteTreatment(this IQueryable<Treatment> treatments, ICollection<Treatment> treatmentsToDelete)
        {
            if(treatmentsToDelete.Any())
            {
                var treatmentIDList = treatmentsToDelete.Select(x => x.TreatmentID).ToList();
                var treatmentsToDeleteFromSourceList = treatments.Where(x => treatmentIDList.Contains(x.TreatmentID)).ToList();

                foreach (var treatmentToDelete in treatmentsToDeleteFromSourceList)
                {
                    treatmentToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteTreatment(this IQueryable<Treatment> treatments, int treatmentID)
        {
            DeleteTreatment(treatments, new List<int> { treatmentID });
        }

        public static void DeleteTreatment(this IQueryable<Treatment> treatments, Treatment treatmentToDelete)
        {
            DeleteTreatment(treatments, new List<Treatment> { treatmentToDelete });
        }
    }
}