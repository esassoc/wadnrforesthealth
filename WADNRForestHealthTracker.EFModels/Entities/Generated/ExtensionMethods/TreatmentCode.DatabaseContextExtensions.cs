//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[TreatmentCode]
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
        public static TreatmentCode GetTreatmentCode(this IQueryable<TreatmentCode> treatmentCodes, int treatmentCodeID)
        {
            var treatmentCode = treatmentCodes.SingleOrDefault(x => x.TreatmentCodeID == treatmentCodeID);
            Check.RequireNotNullThrowNotFound(treatmentCode, "TreatmentCode", treatmentCodeID);
            return treatmentCode;
        }

        // Delete using an IDList (WADNR style)
        public static void DeleteTreatmentCode(this IQueryable<TreatmentCode> treatmentCodes, List<int> treatmentCodeIDList)
        {
            if(treatmentCodeIDList.Any())
            {
                var treatmentCodesInSourceCollectionToDelete = treatmentCodes.Where(x => treatmentCodeIDList.Contains(x.TreatmentCodeID));
                foreach (var treatmentCodeToDelete in treatmentCodesInSourceCollectionToDelete.ToList())
                {
                    treatmentCodeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        // Delete using an object list (WADNR style)
        public static void DeleteTreatmentCode(this IQueryable<TreatmentCode> treatmentCodes, ICollection<TreatmentCode> treatmentCodesToDelete)
        {
            if(treatmentCodesToDelete.Any())
            {
                var treatmentCodeIDList = treatmentCodesToDelete.Select(x => x.TreatmentCodeID).ToList();
                var treatmentCodesToDeleteFromSourceList = treatmentCodes.Where(x => treatmentCodeIDList.Contains(x.TreatmentCodeID)).ToList();

                foreach (var treatmentCodeToDelete in treatmentCodesToDeleteFromSourceList)
                {
                    treatmentCodeToDelete.Delete(HttpRequestStorage.DatabaseEntities);
                }
            }
        }

        public static void DeleteTreatmentCode(this IQueryable<TreatmentCode> treatmentCodes, int treatmentCodeID)
        {
            DeleteTreatmentCode(treatmentCodes, new List<int> { treatmentCodeID });
        }

        public static void DeleteTreatmentCode(this IQueryable<TreatmentCode> treatmentCodes, TreatmentCode treatmentCodeToDelete)
        {
            DeleteTreatmentCode(treatmentCodes, new List<TreatmentCode> { treatmentCodeToDelete });
        }
    }
}